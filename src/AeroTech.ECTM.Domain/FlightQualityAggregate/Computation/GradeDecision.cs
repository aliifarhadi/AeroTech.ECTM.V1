using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public sealed record GradeDecision(QualityGrade Grade, IReadOnlyList<string> Reasons);
}
