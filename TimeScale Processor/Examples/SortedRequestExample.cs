using Swashbuckle.AspNetCore.Filters;
using TimeScale_Processor.DTO;

namespace TimeScale_Processor.Examples
{
    public class SortedRequestExample : IExamplesProvider<FileNameRequest>
    {
        public FileNameRequest GetExamples()
        {
            return new FileNameRequest
            {
                FileName = "test.csv"
            };
        }
    }
}
