using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Npgsql;
using Swashbuckle.AspNetCore.Filters;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using TimeScale_Processor.Context;
using TimeScale_Processor.DTO;
using TimeScale_Processor.Examples;

namespace TimeScale_Processor.Controllers
{
    [ApiController]
    [Route("timeScaleApi/[controller]")]
    public class ValueController : ControllerBase
    {
        private readonly ValuesContext _contextValue;
        private readonly ResultsContext _contextResult;
        private readonly ILogger<ValueController> _logger;

        public ValueController(ValuesContext contextValue, ResultsContext contextResult, ILogger<ValueController> logger)
        {
            _contextValue = contextValue;
            _contextResult = contextResult;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает последние 10 элементов отсортированных по дате по имени заданного файла
        /// </summary>
        /// 
        /// <returns>Список элементов</returns>
        /// <response code="200">Успешное выполнение</response>
        [HttpGet("getLastSorted")]
        [SwaggerRequestExample(typeof(FileNameRequest), typeof(SortedRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SortedResponseExample))]
        public async Task<IEnumerable<Value>> GetLastSortedResults([FromBody] FileNameRequest request)
        {
            try
            {
                int count = request.Count ?? 10;

                var topValues = await _contextValue.Values
                    .OrderByDescending(v => v.Date)
                    .Where(v => v.FileName == request.FileName)
                    .Take(count)
                    .ToListAsync();

                return topValues;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении значений");
                throw;
            }
        }

        /// <summary>
        /// Получает файл, парсит и заносит данные в БД
        /// </summary>
        /// <remarks>
        /// Пример использования:
        /// 
        ///     POST /timeScaleApi/value/read_csv
        ///     Content-Type: multipart/form-data
        ///     
        ///     --boundary
        ///     Content-Disposition: form-data; name="file"; filename="test.csv"
        ///     Content-Type: text/csv
        ///     
        ///     Date;ExecutionTime;Value
        ///     2003-05-12T08-23-15.1234Z;15;24.56
        ///     2005-09-28T14-47-32.5678Z;8;12.34
        ///     2007-11-03T19-12-44.8912Z;23;45.67
        ///     --boundary--
        ///     
        /// </remarks> 
        /// 
        /// <returns>Список элементов</returns>
        [HttpPost("read_csv")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ImportValue), StatusCodes.Status201Created)]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ReadResponseExample))]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ImportValue>> SetValue()
        {
            var form = await Request.ReadFormAsync();

            var file = form.Files.GetFile("file");

            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Файл не выбран" });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".csv")
            {
                return BadRequest(new
                {
                    error = "Неверное расширение файла",
                    message = "Файл должен иметь расширение .csv",
                    receivedExtension = extension
                });
            }

            Console.WriteLine($"Обработка файла: {file.FileName}, размер: {file.Length} байт");

            var validationResult = await ValidateAsync(file);

            if (validationResult.HasErrors)
            {
                return BadRequest(validationResult);
            }

            await DeleteRecordsByFileNameAsync(file.FileName);
            using var transaction = await _contextValue.Database.BeginTransactionAsync();
            try
            {
                await BulkInsertAsync(validationResult.ValidRecords, file.FileName);
                await transaction.CommitAsync();

                return CreatedAtAction(
                    actionName: nameof(SetValue),
                    routeValues: new { fileName = file.FileName }, 
                    value: validationResult 
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Ошибка при сохранении файла {FileName}", file.FileName);

                return StatusCode(500, new
                {
                    error = "Внутренняя ошибка сервера",
                    message = ex.Message
                });
            }
        }

        private async Task<ImportValue> ValidateAsync(IFormFile file)
        {
            var valueResult = new ImportValue();
            var validRecords = new List<ValueDTO>();

            const int maxRecordsInMemory = 10000;

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var reader = new StreamReader(stream);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,
                BadDataFound = null
            };

            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<ValueDTOMap>();

            int rowNumber = 0;

            await csv.ReadAsync();
            csv.ReadHeader();

            await foreach (var record in csv.GetRecordsAsync<ValueDTO>())
            {
                rowNumber++;

                if (rowNumber > maxRecordsInMemory)
                {
                    throw new InvalidOperationException($"Файл слишком большой. Максимум {maxRecordsInMemory} записей");
                }

                var value = MapToValue(record);

                if (ValidateValue(value, out var errors))
                {
                    validRecords.Add(value);
                    valueResult.SuccessCount++;
                }
                else
                {
                    valueResult.Errors.Add($"Строка {rowNumber}: {errors}");
                }
            }

            valueResult.ValidRecords = validRecords;
            valueResult.TotalRecords = rowNumber;

            return valueResult;
        }

        private async Task BulkInsertAsync(List<ValueDTO> records, string fileName)
        {
            var connectionString = _contextValue.Database.GetConnectionString();

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var writer = connection.BeginBinaryImport(
                "COPY \"Values\" (\"Date\", \"ExecutionTime\", \"FileName\", \"Value\") FROM STDIN (FORMAT BINARY)");

            var result = new Result();
            var firstRecord = records.First();
            DateTimeOffset firstData = firstRecord.Date;
            DateTimeOffset lastData = firstRecord.Date;
            double averageExecutionTime = firstRecord.ExecutionTime;
            double averageMetric = firstRecord.Metric;
            int count = records.Count;
            double minMetric = firstRecord.Metric;
            double maxMetric = firstRecord.Metric;


            foreach (var record in records)
            {
                writer.StartRow();
                writer.Write(record.Date, NpgsqlTypes.NpgsqlDbType.TimestampTz);
                writer.Write(record.ExecutionTime, NpgsqlTypes.NpgsqlDbType.Integer);
                writer.Write(fileName, NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(record.Metric, NpgsqlTypes.NpgsqlDbType.Double);

                if (firstData > record.Date) { firstData = record.Date; }
                if (lastData < record.Date) { lastData = record.Date; }
                averageExecutionTime = (averageExecutionTime + record.ExecutionTime) / 2;
                averageMetric = (averageMetric + record.Metric) / 2;
                if (minMetric > record.Metric) { minMetric = record.Metric; }
                if (maxMetric < record.Metric) { maxMetric = record.Metric; }

            }

            writer.Complete();

            records = records.OrderBy(r => r.Metric).ToList();
            double medianMetric;
            if (count % 2 != 0)
            {
                medianMetric = records[(count) / 2].Metric;
            }
            else
            {
                medianMetric = (records[count / 2 - 1].Metric + records[count / 2].Metric) / 2;
            }

            result.FileName = fileName;
            result.DeltaDate = (int)Math.Floor((lastData - firstData).TotalSeconds);
            result.FirstDate = firstData;
            result.AverageExecutionTime = Math.Round(averageExecutionTime, 3);
            result.AverageMetric = Math.Round(averageMetric, 3);
            result.MedianMetric = medianMetric;
            result.MinMetric = minMetric;
            result.MaxMetric = maxMetric;

            _contextResult.Results.Add(result);
            await _contextResult.SaveChangesAsync();
        }

        private ValueDTO MapToValue(ValueDTO value)
        {
            return new ValueDTO
            {
                Date = value.Date,
                ExecutionTime = value.ExecutionTime,
                Metric = value.Metric
            };
        }

        private bool ValidateValue(ValueDTO value, out string errorMessage)
        {
            var errors = new List<string>();

            var now = DateTimeOffset.UtcNow;
            var minDate = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

            if (value.Date > now)
                errors.Add("Время начала не может быть позже текущего");

            if (value.Date < minDate)
                errors.Add("Время начала не может быть раньше 2000-01-01T00-00-00.0000Z");

            if (value.ExecutionTime <= 0)
                errors.Add("Время выполнения должно быть болльше 0");

            if (value.Metric <= 0)
                errors.Add("Показатель должен быть болльше 0");

            errorMessage = string.Join("; ", errors);
            return errors.Count == 0;
        }

        private async Task<int> DeleteRecordsByFileNameAsync(string fileName)
        {
            using var transaction = await _contextValue.Database.BeginTransactionAsync();
            try
            {
                var exists = await _contextValue.Values
                    .AnyAsync(v => v.FileName == fileName);

                if (!exists)
                {
                    _logger.LogWarning("Записей с именем файла {FileName} не найдено", fileName);
                    return 0;
                }

                int deletedCount = await _contextValue.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM \"Values\" WHERE \"FileName\" = {fileName}");

                await _contextResult.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM \"Results\" WHERE \"FileName\" = {fileName}");

                await transaction.CommitAsync();

                _logger.LogInformation("Удалено {Count} записей из Values и соответствующие записи из Results",
                    deletedCount);

                return deletedCount;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Ошибка при удалении записей с именем файла {FileName}", fileName);
                throw;
            }
        }
    }
}
