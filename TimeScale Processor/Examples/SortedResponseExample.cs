using Swashbuckle.AspNetCore.Filters;
using TimeScale_Processor.DTO;

namespace TimeScale_Processor.Examples
{
    public class SortedResponseExample : IExamplesProvider<List<Value>>
    {
        public List<Value> GetExamples()
        {
            return new List<Value>
    {
        new Value
        {
            Id = 486,
            FileName = "test.csv",
            Date = DateTimeOffset.Parse("2017-07-15T10:34:53.0123+00:00"),
            ExecutionTime = 11,
            Metric = 41.05
        },
        new Value
        {
            Id = 473,
            FileName = "test.csv",
            Date = DateTimeOffset.Parse("2016-09-23T12:26:15.1234+00:00"),
            ExecutionTime = 15,
            Metric = 35.62
        },
        new Value
        {
            Id = 455,
            FileName = "test.csv",
            Date = DateTimeOffset.Parse("2016-08-22T11:21:27.4567+00:00"),
            ExecutionTime = 21,
            Metric = 17.33
        },
        new Value
        {
            Id = 442,
            FileName = "test.csv",
            Date = DateTimeOffset.Parse("2015-12-19T13:14:49.5678+00:00"),
            ExecutionTime = 27,
            Metric = 14.95
        },
        new Value
        {
            Id = 461,
            FileName = "test.csv",
            Date = DateTimeOffset.Parse("2015-08-13T19:43:03.6789+00:00"),
            ExecutionTime = 35,
            Metric = 32.74
        },
        new Value
        {
            Id = 414,
            FileName = "test.csv",
            Date = DateTimeOffset.Parse("2015-01-29T01:13:41.0123+00:00"),
            ExecutionTime = 33,
            Metric = 39.51
        },
        new Value
        {
            Id = 479,
            FileName = "test.csv",
            Date = DateTimeOffset.Parse("2015-01-12T20:49:51.4567+00:00"),
            ExecutionTime = 22,
            Metric = 17.02
        },
        new Value
        {
            Id = 420,
            FileName = "test.csv",
            Date = DateTimeOffset.Parse("2014-11-27T14:36:17.2345+00:00"),
            ExecutionTime = 4,
            Metric = 27.89
        },
        new Value
        {
            Id = 448,
            FileName = "test.csv",
            Date = DateTimeOffset.Parse("2014-09-14T21:36:25.789+00:00"),
            ExecutionTime = 34,
            Metric = 9.76
        },
        new Value
        {
            Id = 485,
            FileName = "test.csv",
            Date = DateTimeOffset.Parse("2014-05-02T05:10:27.6789+00:00"),
            ExecutionTime = 34,
            Metric = 31.32
        }
    };
        }
    }
}
