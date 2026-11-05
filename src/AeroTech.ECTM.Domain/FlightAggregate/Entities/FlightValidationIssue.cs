using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Entities
{
    public sealed class FlightValidationIssue : Entity<long>
    {
        private FlightValidationIssue()
        {
        }

        internal FlightValidationIssue(long id, long flightId, ValidationFinding finding)
        {
            Id = id;
            FlightId = flightId;
            Severity = finding.Severity;
            Code = finding.Code;
            Message = finding.Message;
            ParameterKey = finding.ParameterKey;
            Count = finding.Count;
            Value = finding.Value;
        }

        public long FlightId { get; private set; }

        public ValidationSeverity Severity { get; private set; }

        public string Code { get; private set; } = default!;

        public string Message { get; private set; } = default!;

        public string? ParameterKey { get; private set; }

        public long? Count { get; private set; }

        public double? Value { get; private set; }
    }
}
