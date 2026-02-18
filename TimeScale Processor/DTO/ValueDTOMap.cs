using CsvHelper.Configuration;

namespace TimeScale_Processor.DTO
{
    public sealed class ValueDTOMap : ClassMap<ValueDTO>
    {
        public ValueDTOMap()
        {
            Map(m => m.Date)
                .Name("Date")
                .TypeConverter<CustomDateTimeOffsetConverter>();

            Map(m => m.ExecutionTime)
                .Name("ExecutionTime");

            Map(m => m.Metric)
                .Name("Value");
        }
    }
}
