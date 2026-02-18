namespace TimeScale_Processor.DTO
{
    public class ImportValue
    {
        public int TotalRecords { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<ValueDTO> ValidRecords { get; set; } = new();

        public bool HasErrors => Errors != null && Errors.Any();
    }
}
