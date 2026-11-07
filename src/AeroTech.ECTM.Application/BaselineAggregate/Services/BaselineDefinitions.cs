using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Services
{
    public sealed record BaselineDefinitions(
        ConfigurationDocument SnapshotDocument,
        SnapshotRulesContent Snapshot,
        ConfigurationDocument CatalogDocument,
        ValidatedCatalog Catalog)
    {
        public IReadOnlyList<string> EngineParameters => Snapshot.EngineParameters!;

        public IReadOnlyList<string> CruiseSelectors => Snapshot.Cruise!.Selectors!.Select(selector => selector.Code!).ToList();

        public bool IsEngineParameter(string parameterCode) => EngineParameters.Contains(parameterCode);

        public string? UnitOf(string parameterCode) => Catalog.Find(parameterCode)?.Unit;
    }
}
