namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record TimeGap(int AfterSampleIndex, double ElapsedFrom, double ElapsedTo);
}
