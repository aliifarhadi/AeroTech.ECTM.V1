using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.AircraftAggregate
{
    public sealed class Aircraft : AggregateRoot<long>
    {
        private Aircraft()
        {
        }

        private Aircraft(long id, string registration, string? typeCode, string? operatorCode)
        {
            Id = id;
            Registration = registration;
            TypeCode = typeCode;
            OperatorCode = operatorCode;
            EngineCount = EnginePositions.Count;
            Status = AircraftStatus.ACTIVE;
        }

        public string Registration { get; private set; } = default!;

        public string? TypeCode { get; private set; }

        public string? OperatorCode { get; private set; }

        public byte EngineCount { get; private set; }

        public AircraftStatus Status { get; private set; }

        public static Aircraft Register(long id, string registration, string? typeCode, string? operatorCode)
        {
            if (string.IsNullOrWhiteSpace(registration))
                throw ExceptionFactory.AircraftRegistrationIsRequired();

            var normalizedRegistration = registration.Trim().ToUpperInvariant();

            return new Aircraft(id, normalizedRegistration, typeCode?.Trim(), operatorCode?.Trim());
        }

        public void Update(string? typeCode, string? operatorCode, AircraftStatus status)
        {
            TypeCode = typeCode?.Trim();
            OperatorCode = operatorCode?.Trim();
            Status = status;
        }
    }
}
