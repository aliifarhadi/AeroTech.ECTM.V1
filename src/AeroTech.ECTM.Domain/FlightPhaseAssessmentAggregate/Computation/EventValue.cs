namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record EventValue(string ParameterKey, double? AtStart, double? AtEnd, double? Peak);
}
