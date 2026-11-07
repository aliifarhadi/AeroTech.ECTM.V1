using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.RestApi.V1.BaselineAggregate.Requests
{
    public sealed class BaselineEvaluationFilterRequest
    {
        public List<long>? BaselineIds { get; set; }

        public SnapshotKind? SnapshotKind { get; set; }

        public string? Selector { get; set; }

        public string? ParameterCode { get; set; }

        public DateOnly? From { get; set; }

        public DateOnly? To { get; set; }

        public EvaluationFilterArgs ToArgs() => new(BaselineIds, SnapshotKind, Selector, ParameterCode, From, To);
    }
}
