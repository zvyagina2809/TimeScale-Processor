using System.Text.Json.Serialization;

namespace TimeScale_Processor.DTO
{
    public class Filtr
    {
        public string? FileName { get; set; }
        public DateTimeOffset? TimeFirstDataMin { get; set; }
        public DateTimeOffset? TimeFirstDataMax { get; set; }
        [JsonPropertyName("averageValueMin")]
        public double? AverageMetricMin { get; set; }
        [JsonPropertyName("averageValueMax")]
        public double? AverageMetricMax { get; set; }
        public double? AverageExecutionTimeMin { get; set; }
        public double? AverageExecutionTimeMax { get; set; }
    }
}
