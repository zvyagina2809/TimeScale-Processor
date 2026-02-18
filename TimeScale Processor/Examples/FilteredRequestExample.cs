using Swashbuckle.AspNetCore.Filters;
using TimeScale_Processor.DTO;

namespace TimeScale_Processor.Examples
{
    public class FilteredRequestExample : IExamplesProvider<Filtr>
    {
        public Filtr GetExamples()
        {
            return new Filtr
            {
                FileName = "test.csv",
                TimeFirstDataMin = DateTimeOffset.Parse("2000-03-05T07:04:39.8901+03:00"),
                TimeFirstDataMax = DateTimeOffset.Parse("2001-03-05T07:04:39.8901+03:00"),
                AverageMetricMin = 20.000,
                AverageMetricMax = 30.000,
                AverageExecutionTimeMin = 20.000,
                AverageExecutionTimeMax = 30.000
            };
        }
    }
}
