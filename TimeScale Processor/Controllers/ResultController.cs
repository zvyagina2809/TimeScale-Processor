
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Swashbuckle.AspNetCore.Filters;
using System.Linq;
using TimeScale_Processor.Context;
using TimeScale_Processor.DTO;
using TimeScale_Processor.Examples;

namespace TimeScale_Processor.Controllers
{
    [ApiController]
    [Route("timeScaleApi/[controller]")]
    public class ResultController : ControllerBase
    {
        private readonly ResultsContext _contextResult;
        private readonly ILogger<ResultController> _logger;

        public ResultController( ResultsContext contextResult, ILogger<ResultController> logger)
        {
            _contextResult = contextResult;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает список элементов, подходящих под фильтр
        /// </summary>
        /// <returns>Список элементов</returns>
        /// <response code="200">Успешное выполнение</response>
        [HttpGet("getFiltered")]
        [SwaggerRequestExample(typeof(Filtr), typeof(FilteredRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FilteredResponseExample))]
        public async Task<IEnumerable<Result>> GetFilteredResults([FromBody] Filtr filter)
        {
            var query = _contextResult.Results.AsQueryable();

            try
            {
                if (!string.IsNullOrEmpty(filter.FileName))
                    query = query.Where(r => r.FileName.Contains(filter.FileName));

                if (filter.TimeFirstDataMin.HasValue)
                    query = query.Where(r => r.FirstDate >= filter.TimeFirstDataMin.Value);

                if (filter.TimeFirstDataMax.HasValue)
                    query = query.Where(r => r.FirstDate <= filter.TimeFirstDataMax.Value);

                if (filter.AverageMetricMin.HasValue)
                    query = query.Where(r => r.AverageMetric >= filter.AverageMetricMin.Value);

                if (filter.AverageMetricMax.HasValue)
                    query = query.Where(r => r.AverageMetric <= filter.AverageMetricMax.Value);

                if (filter.AverageExecutionTimeMin.HasValue)
                    query = query.Where(r => r.AverageExecutionTime >= filter.AverageExecutionTimeMin.Value);

                if (filter.AverageExecutionTimeMax.HasValue)
                    query = query.Where(r => r.AverageExecutionTime <= filter.AverageExecutionTimeMax.Value);

                return await query
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении значений");
                throw;
            }
        }

        /// <summary>
        /// Возвращает последние 10 элементов отсортированных по начальному времени запуска и имени файла
        /// </summary>
        /// 
        /// <returns>Список элементов</returns>
        /// <response code="200">Успешное выполнение</response>
        [HttpGet("getLastSorted")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FilteredResponseExample))]
        public async Task<IEnumerable<Result>> GetLastSortedResults()
        {
            try
            {
                var topValues = await _contextResult.Results
                    .OrderByDescending(v => v.FirstDate)
                    .ThenByDescending(v => v.FileName)
                    .Take(10)
                    .ToListAsync();

                return topValues;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении значений");
                throw;
            }
        }

    }
}

