using Swashbuckle.AspNetCore.Filters;
using TimeScale_Processor.DTO;

namespace TimeScale_Processor.Examples
{
    public class FilteredResponseExample : IExamplesProvider<Result>
    {
        public Result GetExamples()
        {
            return new Result
            {
                Id = 4,
                FileName = "test.csv",
                DeltaDate = 547885813,
                FirstDate = DateTimeOffset.Parse("2000-05-05T04:04:39.8901+00:00"),
                AverageExecutionTime = 24.848,
                AverageMetric = 22.033,
                MedianMetric = 27.725,
                MinMetric = 3.21,
                MaxMetric = 52.78
            };
        }
    }
}
