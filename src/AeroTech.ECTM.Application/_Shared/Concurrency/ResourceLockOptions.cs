namespace AeroTech.ECTM.Application._Shared.Concurrency
{
    public sealed class ResourceLockOptions
    {
        public const string SectionName = "ResourceLock";

        public int ExpirySeconds { get; set; }
    }
}
