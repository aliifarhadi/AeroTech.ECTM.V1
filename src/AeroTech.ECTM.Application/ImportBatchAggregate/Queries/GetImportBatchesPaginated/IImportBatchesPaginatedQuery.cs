namespace AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchesPaginated
{
    public interface IImportBatchesPaginatedQuery
    {
        int PageNumber { get; }

        int PageSize { get; }
    }
}
