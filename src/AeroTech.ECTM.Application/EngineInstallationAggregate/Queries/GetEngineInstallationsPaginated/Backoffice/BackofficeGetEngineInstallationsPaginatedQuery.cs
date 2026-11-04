using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Queries.GetEngineInstallationsPaginated.Backoffice
{
    public sealed class BackofficeGetEngineInstallationsPaginatedQuery : BackofficePageQuery, IRequest<PaginatedList<BackofficeEngineInstallationDto>>, IEngineInstallationsPaginatedQuery
    {
        public long? EngineId { get; set; }

        public long? AircraftId { get; set; }

        public int? Position { get; set; }

        public bool OpenOnly { get; set; }
    }
}
