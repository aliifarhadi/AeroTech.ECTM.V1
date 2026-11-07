using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Arguments;

namespace AeroTech.ECTM.Domain.BaselineAggregate.Computation
{
    public static class BaselineParameterCheck
    {
        public static void Ensure(IReadOnlyList<BaselineParameterArgs> parameters)
        {
            var problems = ProblemsOf(parameters);

            if (problems.Count > 0)
                throw ExceptionFactory.BaselineParameterInvalid(string.Join(" ", problems));
        }

        public static IReadOnlyList<string> ProblemsOf(IReadOnlyList<BaselineParameterArgs> parameters)
        {
            var problems = new List<string>();
            var measurements = new HashSet<string>(StringComparer.Ordinal);

            foreach (var parameter in parameters)
            {
                var measurement = $"{parameter.SnapshotKind}/{parameter.Selector}/{parameter.ParameterCode}";

                if (!measurements.Add(measurement))
                    problems.Add($"{measurement} is listed twice.");

                if (IsNotFinite(parameter.RawValue))
                    problems.Add($"{measurement} has a value that is not a finite number.");

                if (IsNotFinite(parameter.SuggestedRawValue))
                    problems.Add($"{measurement} has a suggested value that is not a finite number.");
            }

            return problems;
        }

        private static bool IsNotFinite(double? value) => value is { } number && !double.IsFinite(number);
    }
}
