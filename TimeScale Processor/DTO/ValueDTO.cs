using System.Text.Json.Serialization;

namespace TimeScale_Processor.DTO
{
    public class ValueDTO
    {
        public DateTimeOffset Date { get; set; }
        public int ExecutionTime { get; set; }
        [JsonPropertyName("value")]
        public double Metric { get; set; }
    }
}
