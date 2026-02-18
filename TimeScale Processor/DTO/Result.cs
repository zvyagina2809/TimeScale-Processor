using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TimeScale_Processor.DTO
{
    public class Result
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set; }
        public int DeltaDate { get; set; }
        public DateTimeOffset FirstDate { get; set; }
        public double AverageExecutionTime { get; set; }
        [Column("AverageValue")]
        [JsonPropertyName("averageValue")]
        public double AverageMetric { get; set; }
        [Column("MedianValue")]
        [JsonPropertyName("medianValue")]
        public double MedianMetric { get; set; }
        [Column("MinValue")]
        [JsonPropertyName("minValue")]
        public double MinMetric { get; set; }
        [Column("MaxValue")]
        [JsonPropertyName("maxValue")]
        public double MaxMetric { get; set; }
    }
}
