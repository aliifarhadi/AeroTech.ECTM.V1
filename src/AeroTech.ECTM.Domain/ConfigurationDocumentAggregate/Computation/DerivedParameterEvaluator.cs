using System.Globalization;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class DerivedParameterEvaluator
    {
        public const string Available = "AVAILABLE";
        public const string PositionRequired = "POSITION_REQUIRED";
        public const string MissingInput = "MISSING_INPUT";
        public const string UnitMismatch = "UNIT_MISMATCH";
        public const string NullInput = "NULL_INPUT";
        public const string DenominatorZero = "DENOMINATOR_ZERO";
        public const string NonFinite = "NON_FINITE";

        public static TypedEvaluation Evaluate(
            DerivedParameterDefinition definition,
            IReadOnlyDictionary<string, CatalogEntry> entries,
            int? position,
            IReadOnlyDictionary<string, TypedInput> inputs)
        {
            var references = ReferencesOf(definition);
            var usesPosition = references.Any(reference => reference.UsesPositionPlaceholder);
            var code = ResultCode(definition.Code!, usesPosition, position);

            if (usesPosition && position is not (1 or 2))
                return Unavailable(definition, code, PositionRequired, ["A position of 1 or 2 is required for a [pos] definition."], []);

            var evaluatedInputs = new List<EvaluatedInput>();
            var problems = new List<(string Status, string Reason)>();

            foreach (var reference in references)
            {
                var resolved = reference.Resolve(position);
                var expectedUnit = entries[reference.Code].Unit;

                if (!inputs.TryGetValue(resolved, out var input))
                {
                    evaluatedInputs.Add(new EvaluatedInput(resolved, null, expectedUnit));
                    problems.Add((MissingInput, $"{resolved} was not supplied."));
                    continue;
                }

                evaluatedInputs.Add(new EvaluatedInput(resolved, input.Value, input.Unit));

                if (input.Unit != expectedUnit)
                    problems.Add((UnitMismatch, $"{resolved} is in {input.Unit}; the catalog unit is {expectedUnit}."));
                else if (input.Value is null)
                    problems.Add((NullInput, $"{resolved} has no value."));
            }

            if (problems.Count > 0)
                return Unavailable(definition, code, problems[0].Status, problems.Select(problem => problem.Reason).ToList(), evaluatedInputs);

            var values = evaluatedInputs.Select(input => input.Value!.Value).ToList();

            return definition.Kind switch
            {
                DerivedParameterKinds.Difference => Difference(definition, code, evaluatedInputs, values),
                DerivedParameterKinds.Ratio => Ratio(definition, code, evaluatedInputs, values),
                DerivedParameterKinds.LinearCombination => LinearCombination(definition, code, evaluatedInputs, values),
                _ => PowerRatio(definition, code, evaluatedInputs, values)
            };
        }

        public static IReadOnlyList<ParameterReference> ReferencesOf(DerivedParameterDefinition definition)
        {
            string?[] texts = definition.Kind switch
            {
                DerivedParameterKinds.Difference => [definition.A, definition.B],
                DerivedParameterKinds.Ratio => [definition.Numerator, definition.Denominator],
                DerivedParameterKinds.LinearCombination => (definition.Terms ?? []).Select(term => term.Ref).ToArray(),
                _ => [definition.Ref]
            };

            return texts.Select(text => ParameterReference.Parse(text)!).ToList();
        }

        private static TypedEvaluation Difference(DerivedParameterDefinition definition, string code, IReadOnlyList<EvaluatedInput> inputs, IReadOnlyList<double> values)
        {
            var result = values[0] - values[1];
            var formula = $"{inputs[0].Ref} - {inputs[1].Ref} = {Text(values[0])} - {Text(values[1])} = {Text(result)}";

            return Finished(definition, code, result, inputs, new Dictionary<string, double>(), formula);
        }

        private static TypedEvaluation Ratio(DerivedParameterDefinition definition, string code, IReadOnlyList<EvaluatedInput> inputs, IReadOnlyList<double> values)
        {
            var numerator = values[0];
            var denominator = values[1];

            if (denominator == 0)
                return Unavailable(definition, code, DenominatorZero, [$"{inputs[1].Ref} is zero."], inputs);

            var result = numerator / denominator;
            var formula = $"{inputs[0].Ref} / {inputs[1].Ref} = {Text(numerator)} / {Text(denominator)} = {Text(result)}";

            return Finished(definition, code, result, inputs, new Dictionary<string, double>(), formula);
        }

        private static TypedEvaluation LinearCombination(DerivedParameterDefinition definition, string code, IReadOnlyList<EvaluatedInput> inputs, IReadOnlyList<double> values)
        {
            var terms = definition.Terms!;
            var offset = definition.Offset!.Value;
            var constants = new Dictionary<string, double> { ["offset"] = offset };
            var symbolic = new List<string>();
            var substituted = new List<string>();
            var result = offset;

            for (var index = 0; index < terms.Count; index++)
            {
                var coefficient = terms[index].Coefficient!.Value;
                constants[$"terms[{index}].coefficient"] = coefficient;
                result += coefficient * values[index];

                symbolic.Add($"{Text(coefficient)}*{inputs[index].Ref}");
                substituted.Add($"{Text(coefficient)}*{Text(values[index])}");
            }

            var formula = $"{string.Join(" + ", symbolic)} + {Text(offset)} = {string.Join(" + ", substituted)} + {Text(offset)} = {Text(result)}";

            return Finished(definition, code, result, inputs, constants, formula);
        }

        private static TypedEvaluation PowerRatio(DerivedParameterDefinition definition, string code, IReadOnlyList<EvaluatedInput> inputs, IReadOnlyList<double> values)
        {
            var referenceConstant = definition.ReferenceConstant!.Value;
            var result = values[0] / referenceConstant;
            var constants = new Dictionary<string, double> { ["referenceConstant"] = referenceConstant };
            var formula = $"{inputs[0].Ref} / {Text(referenceConstant)} = {Text(values[0])} / {Text(referenceConstant)} = {Text(result)}";

            return Finished(definition, code, result, inputs, constants, formula);
        }

        private static TypedEvaluation Finished(
            DerivedParameterDefinition definition,
            string code,
            double result,
            IReadOnlyList<EvaluatedInput> inputs,
            IReadOnlyDictionary<string, double> constants,
            string formula)
        {
            if (!double.IsFinite(result))
                return Unavailable(definition, code, NonFinite, ["The result is not a finite number."], inputs);

            return new TypedEvaluation(code, definition.Kind!, result, definition.UnitOut, Available, [], inputs, constants, formula);
        }

        private static TypedEvaluation Unavailable(
            DerivedParameterDefinition definition,
            string code,
            string status,
            IReadOnlyList<string> reasons,
            IReadOnlyList<EvaluatedInput> inputs)
            => new(code, definition.Kind!, null, definition.UnitOut, status, reasons, inputs, new Dictionary<string, double>(), null);

        private static string ResultCode(string code, bool usesPosition, int? position)
        {
            if (usesPosition && position is { } enginePosition)
                return $"{code}[{enginePosition}]";

            return code;
        }

        private static string Text(double value) => value.ToString("R", CultureInfo.InvariantCulture);
    }
}
