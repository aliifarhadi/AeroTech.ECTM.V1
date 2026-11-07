using AeroTech.ECTM.Domain.BaselineAggregate.Computation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Dto
{
    public sealed record BackofficeConditionValueDto(string ParameterCode, double Value)
    {
        public static IReadOnlyList<BackofficeConditionValueDto> ListOf(IEnumerable<ConditionValue> values)
            => values.Select(value => new BackofficeConditionValueDto(value.ParameterCode, value.Value)).ToList();

        public static IReadOnlyList<BackofficeConditionValueDto> ListOf(MeasurementContext context)
            => ListOf(context.Conditions.Concat(context.EngineValues));
    }
}
