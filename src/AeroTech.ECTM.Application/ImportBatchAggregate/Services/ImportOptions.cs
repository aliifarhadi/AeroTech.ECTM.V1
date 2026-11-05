namespace AeroTech.ECTM.Application.ImportBatchAggregate.Services
{
    public sealed class ImportOptions
    {
        public const string SectionName = "Import";

        public const long RequestBodyLimitBytes = 1_073_741_824;

        public long MaximumFileBytes { get; set; } = 50_000_000;

        public int MaximumBatchFiles { get; set; } = 200;

        public long MaximumBatchBytes { get; set; } = 1_073_741_824;

        public bool SyntheticSourcesAllowed { get; set; }
    }
}
