namespace AeroTech.ECTM.RestApi.V1.EngineModelAggregate.Requests
{
    public sealed record UpdateEngineModelRequest(
        string Manufacturer,
        string? Family,
        string? VariantCode,
        string? ThrustRatingCode,
        IReadOnlyList<string>? DocumentReferences,
        string? Notes);
}
