namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.CreateEngineModel
{
    public interface ICreateEngineModelCommand
    {
        string Code { get; }

        string Manufacturer { get; }

        string? Family { get; }

        string? VariantCode { get; }

        string? ThrustRatingCode { get; }

        IReadOnlyList<string> DocumentReferences { get; }

        string? Notes { get; }
    }
}
