using Microsoft.EntityFrameworkCore;
using TimeScale_Processor.DTO;

namespace TimeScale_Processor.Context
{
    public class ValuesContext : DbContext
    {
        public DbSet<Value> Values { get; set; }

        public ValuesContext(DbContextOptions<ValuesContext> options)
        : base(options)
        {
        }
    }
}
