using Swashbuckle.AspNetCore.Filters;
using TimeScale_Processor.DTO;

namespace TimeScale_Processor.Examples
{
    public class ReadResponseExample : IExamplesProvider<ImportValue>
    {
        public ImportValue GetExamples()
        {
            return new ImportValue
            {
                TotalRecords = 98,
                SuccessCount = 98,
                ErrorCount = 0,
                Errors = new List<string>(), 
                ValidRecords = new List<ValueDTO>
            {
                new ValueDTO
                {
                    Date = DateTimeOffset.Parse("2003-05-12T08:23:15.1234+00:00"),
                    ExecutionTime = 15,
                    Metric = 24.56
                },
                new ValueDTO
                {
                    Date = DateTimeOffset.Parse("2005-09-28T14:47:32.5678+00:00"),
                    ExecutionTime = 8,
                    Metric = 12.34
                },
                new ValueDTO
                {
                    Date = DateTimeOffset.Parse("2007-11-03T19:12:44.8912+00:00"),
                    ExecutionTime = 23,
                    Metric = 45.67
                },
                new ValueDTO
                {
                    Date = DateTimeOffset.Parse("2010-02-18T06:35:21.3456+00:00"),
                    ExecutionTime = 12,
                    Metric = 18.92
                },
                new ValueDTO
                {
                    Date = DateTimeOffset.Parse("2012-07-24T11:58:09.6789+00:00"),
                    ExecutionTime = 31,
                    Metric = 37.41
                },
                new ValueDTO
                {
                    Date = DateTimeOffset.Parse("2001-08-09T22:41:53.0123+00:00"),
                    ExecutionTime = 5,
                    Metric = 5.23
                },
                new ValueDTO
                {
                    Date = DateTimeOffset.Parse("2004-12-15T03:14:27.4567+00:00"),
                    ExecutionTime = 42,
                    Metric = 52.78
                },
                new ValueDTO
                {
                    Date = DateTimeOffset.Parse("2006-04-30T17:39:18.789+00:00"),
                    ExecutionTime = 9,
                    Metric = 8.45
                },
                new ValueDTO
                {
                    Date = DateTimeOffset.Parse("2009-09-05T09:52:44.1234+00:00"),
                    ExecutionTime = 27,
                    Metric = 33.19
                },
                new ValueDTO
                {
                    Date = DateTimeOffset.Parse("2011-01-20T13:26:31.5678+00:00"),
                    ExecutionTime = 18,
                    Metric = 21.63
                }
            }
            };
        }
    }
}
