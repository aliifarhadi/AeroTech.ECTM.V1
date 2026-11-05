namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record StuckChannelRule(
        string? ParameterCode,
        double? MinConstantSec,
        double? MinN1ChangePct);
}
