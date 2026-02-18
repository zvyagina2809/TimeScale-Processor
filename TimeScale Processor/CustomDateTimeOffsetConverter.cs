using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;

namespace TimeScale_Processor
{
    public class CustomDateTimeOffsetConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
                return DateTimeOffset.MinValue;

            try
            {
                _ = text.Trim();

                int tIndex = text.IndexOf('T');
                if (tIndex < 0)
                    throw new FormatException($"Неверный формат даты: {text}");

                string datePart = text.Substring(0, tIndex); 
                string timePart = text.Substring(tIndex + 1); 

                timePart = timePart.TrimEnd('Z');

                string[] timeSections = timePart.Split('.');
                string hmsPart = timeSections[0]; 
                string msPart = timeSections.Length > 1 ? timeSections[1] : "0"; 

                string[] hmsValues = hmsPart.Split('-');
                if (hmsValues.Length != 3)
                    throw new FormatException($"Неверный формат времени: {hmsPart}");

                string correctedTimePart = $"{hmsValues[0]}:{hmsValues[1]}:{hmsValues[2]}.{msPart}";
                string correctedDate = $"{datePart}T{correctedTimePart}Z";

                bool parsed = DateTimeOffset.TryParse(correctedDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset result);

                if (!parsed)
                {
                    result = ParseDateTimeOffsetManually(datePart, hmsValues, msPart);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new TypeConverterException(this, memberMapData, text, row.Context, ex.Message);
            }
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is DateTimeOffset dto)
            {
                return dto.ToString("yyyy-MM-ddTHH-mm-ss.ffffZ");
            }
            return string.Empty;
        }

        private DateTimeOffset ParseDateTimeOffsetManually(string datePart, string[] hmsValues, string msPart)
        {
            try
            {
                int year = int.Parse(datePart.Substring(0, 4));
                int month = int.Parse(datePart.Substring(5, 2));
                int day = int.Parse(datePart.Substring(8, 2));

                int hour = int.Parse(hmsValues[0]);
                int minute = int.Parse(hmsValues[1]);
                int second = int.Parse(hmsValues[2]);

                msPart = msPart.PadRight(3, '0');
                if (msPart.Length > 3)
                    msPart = msPart.Substring(0, 3);
                int milliseconds = int.Parse(msPart);

                var dateTime = new DateTime(year, month, day, hour, minute, second, milliseconds, DateTimeKind.Utc);
                return new DateTimeOffset(dateTime, TimeSpan.Zero);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Ошибка ручного парсинга даты: {ex.Message}");
            }
        }
    }
}
