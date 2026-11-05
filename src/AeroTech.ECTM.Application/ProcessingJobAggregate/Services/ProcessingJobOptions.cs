namespace AeroTech.ECTM.Application.ProcessingJobAggregate.Services
{
    public sealed class ProcessingJobOptions
    {
        public const string SectionName = "ProcessingJobs";

        public int LeaseMinutes { get; set; } = 10;

        public int PollIntervalSeconds { get; set; } = 2;
    }
}
