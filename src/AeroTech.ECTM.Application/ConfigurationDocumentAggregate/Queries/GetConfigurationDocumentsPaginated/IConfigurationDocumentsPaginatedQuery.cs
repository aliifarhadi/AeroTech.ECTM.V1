namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentsPaginated
{
    public interface IConfigurationDocumentsPaginatedQuery
    {
        string? Kind { get; }

        int PageNumber { get; }

        int PageSize { get; }
    }
}
