namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record ModeObservation(string ParameterKey, string Value, int FirstIndex, int Samples);
}
