using Microsoft.EntityFrameworkCore;
using TimeScale_Processor.DTO;

namespace TimeScale_Processor.Context
{
    public class ResultsContext : DbContext
    {
        public DbSet<Result> Results { get; set; }

        public ResultsContext(DbContextOptions<ResultsContext> options)
        : base(options)
        {
        }
    }
}
