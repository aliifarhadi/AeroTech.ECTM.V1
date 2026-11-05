namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record EncodedChannel(byte[] Payload, string Sha256);
}
