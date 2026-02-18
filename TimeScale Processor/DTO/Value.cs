using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TimeScale_Processor.DTO
{
    public class Value
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTimeOffset Date { get; set; }
        public int ExecutionTime { get; set; }
        [Column("Value")]
        [JsonPropertyName("value")]
        public double Metric { get; set; }

    }
}
