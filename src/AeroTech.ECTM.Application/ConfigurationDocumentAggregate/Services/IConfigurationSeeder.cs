namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public interface IConfigurationSeeder
    {
        Task SeedMissingAsync(CancellationToken cancellationToken = default);
    }
}
