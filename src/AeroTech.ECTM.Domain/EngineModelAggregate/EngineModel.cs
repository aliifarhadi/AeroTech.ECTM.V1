using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Framework.Core.Domain.Aggregates;

namespace AeroTech.ECTM.Domain.EngineModelAggregate
{
    public sealed class EngineModel : AggregateRoot<long>
    {
        private EngineModel()
        {
        }

        private EngineModel(long id, string code)
        {
            Id = id;
            Code = code;
        }

        public string Code { get; private set; } = default!;

        public string Manufacturer { get; private set; } = default!;

        public string? Family { get; private set; }

        public string? VariantCode { get; private set; }

        public string? ThrustRatingCode { get; private set; }

        public List<string> DocumentReferences { get; private set; } = [];

        public string? Notes { get; private set; }

        public static EngineModel Register(
            long id,
            string code,
            string manufacturer,
            string? family,
            string? variantCode,
            string? thrustRatingCode,
            IReadOnlyList<string> documentReferences,
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw ExceptionFactory.EngineModelIsIncomplete();

            var engineModel = new EngineModel(id, code.Trim());
            engineModel.Update(manufacturer, family, variantCode, thrustRatingCode, documentReferences, notes);

            return engineModel;
        }

        public void Update(
            string manufacturer,
            string? family,
            string? variantCode,
            string? thrustRatingCode,
            IReadOnlyList<string> documentReferences,
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(manufacturer))
                throw ExceptionFactory.EngineModelIsIncomplete();

            Manufacturer = manufacturer.Trim();
            Family = family?.Trim();
            VariantCode = variantCode?.Trim();
            ThrustRatingCode = thrustRatingCode?.Trim();
            DocumentReferences = documentReferences.Select(reference => reference.Trim()).ToList();
            Notes = notes?.Trim();
        }
    }
}
