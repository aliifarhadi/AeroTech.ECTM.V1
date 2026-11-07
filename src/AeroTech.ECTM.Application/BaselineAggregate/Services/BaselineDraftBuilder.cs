using AeroTech.ECTM.Application.BaselineAggregate.Commands;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Arguments;
using AeroTech.ECTM.Domain.BaselineAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineAggregate.Services
{
    public static class BaselineDraftBuilder
    {
        public static BaselineArgs ArgsOf(IBaselineDraftInput input)
            => new(
                input.Name,
                input.DeclaredEffectiveDate,
                input.ShopVisitEventId,
                input.FirstPostShopDeclared,
                input.SourceFileName,
                input.SourceFileSha256,
                input.Notes);

        public static IReadOnlyList<BaselineParameterArgs> ParametersOf(IReadOnlyList<BaselineParameterInput> inputs, BaselineDefinitions definitions)
        {
            var problems = new List<string>();

            foreach (var input in inputs)
                problems.AddRange(ProblemsOf(input, definitions));

            if (problems.Count > 0)
                throw ExceptionFactory.BaselineParameterInvalid(string.Join(" ", problems));

            return inputs.Select(input => ArgsOf(input, definitions)).ToList();
        }

        private static IEnumerable<string> ProblemsOf(BaselineParameterInput input, BaselineDefinitions definitions)
        {
            if (!definitions.IsEngineParameter(input.ParameterCode))
                yield return $"{input.ParameterCode} is not an engine parameter of the active SNAPSHOT_RULES ({string.Join(", ", definitions.EngineParameters)}).";

            if (input.SnapshotKind == SnapshotKind.CRUISE && !definitions.CruiseSelectors.Contains(input.Selector ?? string.Empty))
                yield return $"A CRUISE value needs one of the cruise selectors {string.Join(", ", definitions.CruiseSelectors)}; '{input.Selector}' is not one.";

            if (input.SnapshotKind != SnapshotKind.CRUISE && input.Selector is not null)
                yield return $"Only a CRUISE value names a selector; {input.SnapshotKind} has none.";
        }

        private static BaselineParameterArgs ArgsOf(BaselineParameterInput input, BaselineDefinitions definitions)
            => new(
                input.SnapshotKind,
                input.Selector,
                input.ParameterCode,
                definitions.UnitOf(input.ParameterCode),
                input.RawValue,
                input.SuggestedRawValue,
                input.SourceEnginePosition,
                input.SourceStartIndex,
                input.SourceEndIndexInclusive,
                input.SourcePeakIndex,
                ReferenceContextOf(input, definitions),
                definitions.CatalogDocument.Id,
                definitions.CatalogDocument.VersionNumber,
                definitions.SnapshotDocument.Id,
                definitions.SnapshotDocument.VersionNumber,
                input.Notes);

        private static MeasurementContext ReferenceContextOf(BaselineParameterInput input, BaselineDefinitions definitions)
        {
            var conditions = new List<ConditionValue>();
            var engineValues = new List<ConditionValue>();

            foreach (var condition in input.ReferenceConditions ?? [])
            {
                var value = new ConditionValue(condition.ParameterCode, condition.Value);

                if (definitions.IsEngineParameter(condition.ParameterCode))
                    engineValues.Add(value);
                else
                    conditions.Add(value);
            }

            var configuration = string.IsNullOrWhiteSpace(input.ReferenceConfiguration) ? null : input.ReferenceConfiguration.Trim();

            return new MeasurementContext(conditions, engineValues, configuration);
        }
    }
}
