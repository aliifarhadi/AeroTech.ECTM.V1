using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Entities;

namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeEngineAssignmentDto(
        int Position,
        EnumValueDto Assignment,
        long? EngineId,
        string? EngineSerialNumber,
        long? InstallationId)
    {
        public static BackofficeEngineAssignmentDto From(EngineResolution engine, IReadOnlyDictionary<long, string> serialByEngine)
            => new(
                engine.Position,
                EnumValueDto.Of(engine.Status),
                engine.EngineId,
                SerialOf(engine.EngineId, serialByEngine),
                engine.InstallationId);

        public static BackofficeEngineAssignmentDto From(FlightEngineAssignment engine, IReadOnlyDictionary<long, string> serialByEngine)
            => new(
                engine.Position,
                EnumValueDto.Of(engine.Status),
                engine.EngineId,
                SerialOf(engine.EngineId, serialByEngine),
                engine.InstallationId);

        private static string? SerialOf(long? engineId, IReadOnlyDictionary<long, string> serialByEngine)
        {
            if (engineId is not { } id)
                return null;

            return serialByEngine.GetValueOrDefault(id);
        }
    }
}
