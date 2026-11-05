using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Commands.EvaluateFlightQuality.Backoffice
{
    public sealed record BackofficeEvaluateFlightQualityCommand(long FlightId) : IRequest<BackofficeFlightQualityDto>;
}
