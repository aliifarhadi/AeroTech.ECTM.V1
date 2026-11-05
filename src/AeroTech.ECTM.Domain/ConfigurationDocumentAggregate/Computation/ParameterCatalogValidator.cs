using System.Text.Json;
using System.Text.RegularExpressions;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class ParameterCatalogValidator
    {
        public const string CatalogCodeRequired = "CATALOG_CODE_REQUIRED";
        public const string ParametersRequired = "PARAMETERS_REQUIRED";
        public const string CodeInvalid = "CODE_INVALID";
        public const string CodeDuplicate = "CODE_DUPLICATE";
        public const string RoleUnknown = "ROLE_UNKNOWN";
        public const string UnitUnknown = "UNIT_UNKNOWN";
        public const string ValueKindUnknown = "VALUE_KIND_UNKNOWN";
        public const string PositionsRequired = "POSITIONS_REQUIRED";
        public const string PositionInvalid = "POSITION_INVALID";
        public const string PlausibilityInvalid = "PLAUSIBILITY_INVALID";
        public const string EnumValuesInvalid = "ENUM_VALUES_INVALID";
        public const string OemCitationRequired = "OEM_CITATION_REQUIRED";
        public const string KindUnknown = "KIND_UNKNOWN";
        public const string KindNotAvailable = "KIND_NOT_AVAILABLE";
        public const string FieldNotAllowed = "FIELD_NOT_ALLOWED";
        public const string FieldRequired = "FIELD_REQUIRED";
        public const string StatusUnknown = "STATUS_UNKNOWN";
        public const string RefInvalid = "REF_INVALID";
        public const string RefUnknown = "REF_UNKNOWN";
        public const string RefPositionInvalid = "REF_POSITION_INVALID";
        public const string RefNotNumeric = "REF_NOT_NUMERIC";
        public const string UnitsDoNotReconcile = "UNITS_DO_NOT_RECONCILE";
        public const string ConstantInvalid = "CONSTANT_INVALID";
        public const string ExponentRequiresCorrectionModel = "EXPONENT_REQUIRES_CORRECTION_MODEL";

        public static readonly IReadOnlySet<string> Roles = new HashSet<string>(StringComparer.Ordinal)
        {
            "TIME_AXIS",
            "CLOCK",
            "IDENTITY",
            "ENGINE_PERFORMANCE",
            "ENGINE_HEALTH",
            "ENGINE_CONTROL",
            "CONDITION",
            "CONFIGURATION",
            "EVENT",
            "DISCRETE_STATE",
            "RECORDER",
            "DERIVED_SOURCE",
            "CORRECTION"
        };

        public static readonly IReadOnlySet<string> ValueKinds = new HashSet<string>(StringComparer.Ordinal)
        {
            "NUMERIC",
            "INTEGER",
            "ENUM",
            "TEXT"
        };

        public static readonly IReadOnlySet<string> DerivedStatuses = new HashSet<string>(StringComparer.Ordinal)
        {
            "CANDIDATE",
            "CONFIRMED"
        };

        private static readonly string[] CommonDerivedFields = ["code", "kind", "role", "unitOut", "status", "description"];

        private static readonly Regex CodePattern = new("^[A-Z][A-Z0-9_]*$", RegexOptions.Compiled);

        private static readonly JsonSerializerOptions DefinitionOptions = new(JsonSerializerDefaults.Web);

        public static CatalogValidation Validate(string content)
        {
            var issues = new List<ValidationIssue>();
            var document = ContentJson.Read<ParameterCatalogContent>(content, issues);

            if (document is null)
                return new CatalogValidation(null, issues);

            if (string.IsNullOrWhiteSpace(document.CatalogCode))
                issues.Add(new ValidationIssue("$.catalogCode", CatalogCodeRequired, "A catalog code is required."));

            var entries = new Dictionary<string, CatalogEntry>(StringComparer.Ordinal);
            ValidateParameters(document.Parameters, entries, issues);

            var derivedParameters = new List<DerivedParameterDefinition>();
            var definitions = document.DerivedParameters ?? [];

            for (var index = 0; index < definitions.Count; index++)
            {
                var validation = ValidateDefinition(definitions[index], entries, $"$.derivedParameters[{index}]", allowExistingCode: false);
                issues.AddRange(validation.Issues);

                if (validation.Definition is null || validation.Entry is null)
                    continue;

                derivedParameters.Add(validation.Definition);
                entries.TryAdd(validation.Entry.Code, validation.Entry);
            }

            return new CatalogValidation(new ValidatedCatalog(entries, derivedParameters), issues);
        }

        public static DefinitionValidation ValidateDefinition(
            JsonElement element,
            IReadOnlyDictionary<string, CatalogEntry> entries,
            string path,
            bool allowExistingCode)
        {
            var issues = new List<ValidationIssue>();

            if (element.ValueKind != JsonValueKind.Object)
            {
                issues.Add(new ValidationIssue(path, ContentJson.ShapeInvalid, "A derived parameter must be a JSON object."));
                return new DefinitionValidation(null, null, issues);
            }

            var kindCode = element.TryGetProperty("kind", out var kindElement) && kindElement.ValueKind == JsonValueKind.String
                ? kindElement.GetString()
                : null;
            var kind = DerivedParameterKinds.Find(kindCode);

            if (kind is null)
            {
                issues.Add(new ValidationIssue($"{path}.kind", KindUnknown, $"Kind '{kindCode}' is not a typed derived-parameter kind."));
                return new DefinitionValidation(null, null, issues);
            }

            if (!kind.IsAvailableNow)
            {
                issues.Add(new ValidationIssue($"{path}.kind", KindNotAvailable, $"Kind {kind.Code} is delivered in {kind.AvailableFrom}."));
                return new DefinitionValidation(null, null, issues);
            }

            foreach (var property in element.EnumerateObject())
            {
                var allowed = CommonDerivedFields.Contains(property.Name) || kind.Fields.Contains(property.Name);

                if (!allowed)
                    issues.Add(new ValidationIssue($"{path}.{property.Name}", FieldNotAllowed, $"Field '{property.Name}' is not part of kind {kind.Code}."));
            }

            if (issues.Count > 0)
                return new DefinitionValidation(null, null, issues);

            var definition = element.Deserialize<DerivedParameterDefinition>(DefinitionOptions)!;
            var checker = new DefinitionChecker(entries, path, issues);

            checker.CheckHeader(definition, allowExistingCode);

            var references = kind.Code switch
            {
                DerivedParameterKinds.Difference => checker.CheckDifference(definition),
                DerivedParameterKinds.Ratio => checker.CheckRatio(definition),
                DerivedParameterKinds.LinearCombination => checker.CheckLinearCombination(definition),
                _ => checker.CheckPowerRatio(definition)
            };

            if (issues.Count > 0)
                return new DefinitionValidation(definition, null, issues);

            var entry = new CatalogEntry(definition.Code!, definition.UnitOut, "NUMERIC", PositionsOf(references, entries), IsDerived: true);

            return new DefinitionValidation(definition, entry, issues);
        }

        private static void ValidateParameters(IReadOnlyList<CatalogParameter>? parameters, Dictionary<string, CatalogEntry> entries, List<ValidationIssue> issues)
        {
            if (parameters is null || parameters.Count == 0)
            {
                issues.Add(new ValidationIssue("$.parameters", ParametersRequired, "The catalog needs at least one parameter."));
                return;
            }

            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                var path = $"$.parameters[{index}]";

                if (parameter.Code is null || !CodePattern.IsMatch(parameter.Code))
                    issues.Add(new ValidationIssue($"{path}.code", CodeInvalid, $"Code '{parameter.Code}' must be UPPER_SNAKE."));
                else if (entries.ContainsKey(parameter.Code))
                    issues.Add(new ValidationIssue($"{path}.code", CodeDuplicate, $"Code {parameter.Code} is defined twice."));

                if (parameter.Role is null || !Roles.Contains(parameter.Role))
                    issues.Add(new ValidationIssue($"{path}.role", RoleUnknown, $"Role '{parameter.Role}' is unknown."));

                if (parameter.Unit is null || !UnitCodes.Known.Contains(parameter.Unit))
                    issues.Add(new ValidationIssue($"{path}.unit", UnitUnknown, $"Unit '{parameter.Unit}' is unknown."));

                if (parameter.ValueKind is null || !ValueKinds.Contains(parameter.ValueKind))
                    issues.Add(new ValidationIssue($"{path}.valueKind", ValueKindUnknown, $"Value kind '{parameter.ValueKind}' is unknown."));

                var positions = ValidPositions(parameter.Positions, $"{path}.positions", issues);
                CheckPlausibility(parameter, path, issues);
                CheckEnumValues(parameter, path, issues);
                CheckOemIndication(parameter.OemIndication, $"{path}.oemIndication", issues);

                if (parameter.Code is not null && CodePattern.IsMatch(parameter.Code))
                    entries.TryAdd(parameter.Code, new CatalogEntry(parameter.Code, parameter.Unit, parameter.ValueKind, positions, IsDerived: false, parameter.Plausibility, parameter.EnumValues));
            }
        }

        private static IReadOnlyList<int> ValidPositions(IReadOnlyList<int>? positions, string path, List<ValidationIssue> issues)
        {
            if (positions is null)
            {
                issues.Add(new ValidationIssue(path, PositionsRequired, "Positions are required; use [] for an aircraft-level parameter."));
                return [];
            }

            var distinct = positions.Distinct().ToList();
            var allValid = positions.All(position => position is 1 or 2);

            if (!allValid || distinct.Count != positions.Count)
                issues.Add(new ValidationIssue(path, PositionInvalid, "Positions must be distinct values of 1 and 2."));

            return distinct.Where(position => position is 1 or 2).Order().ToList();
        }

        private static void CheckPlausibility(CatalogParameter parameter, string path, List<ValidationIssue> issues)
        {
            if (parameter.Plausibility is not { } range)
                return;

            var numeric = parameter.ValueKind is "NUMERIC" or "INTEGER";

            if (!numeric)
                issues.Add(new ValidationIssue($"{path}.plausibility", PlausibilityInvalid, "A plausibility range applies to numeric parameters only."));

            if (range.Min is not { } min || range.Max is not { } max || min >= max)
                issues.Add(new ValidationIssue($"{path}.plausibility", PlausibilityInvalid, "A plausibility range needs min < max."));
        }

        private static void CheckEnumValues(CatalogParameter parameter, string path, List<ValidationIssue> issues)
        {
            if (parameter.EnumValues is not { } values)
                return;

            if (parameter.ValueKind != "ENUM")
                issues.Add(new ValidationIssue($"{path}.enumValues", EnumValuesInvalid, "Enumerated values apply to ENUM parameters only."));

            var blank = values.Any(string.IsNullOrWhiteSpace);
            var distinctCount = values.Select(value => value.Trim()).Distinct(StringComparer.Ordinal).Count();

            if (values.Count == 0 || blank || distinctCount != values.Count)
                issues.Add(new ValidationIssue($"{path}.enumValues", EnumValuesInvalid, "Enumerated values must be distinct, non-blank literals."));
        }

        private static void CheckOemIndication(OemIndication? indication, string path, List<ValidationIssue> issues)
        {
            if (indication is null)
                return;

            if (string.IsNullOrWhiteSpace(indication.Text) || string.IsNullOrWhiteSpace(indication.Source))
                issues.Add(new ValidationIssue(path, OemCitationRequired, "An OEM indication needs its text and its source citation."));
        }

        private static IReadOnlyList<int> PositionsOf(IReadOnlyList<ParameterReference> references, IReadOnlyDictionary<string, CatalogEntry> entries)
        {
            var placeholderReferences = references.Where(reference => reference.UsesPositionPlaceholder).ToList();

            if (placeholderReferences.Count == 0)
                return [];

            IEnumerable<int> positions = [1, 2];

            foreach (var reference in placeholderReferences)
                positions = positions.Intersect(entries[reference.Code].Positions);

            return positions.Order().ToList();
        }

        private sealed class DefinitionChecker(IReadOnlyDictionary<string, CatalogEntry> entries, string path, List<ValidationIssue> issues)
        {
            public void CheckHeader(DerivedParameterDefinition definition, bool allowExistingCode)
            {
                if (definition.Code is null || !CodePattern.IsMatch(definition.Code))
                    issues.Add(new ValidationIssue($"{path}.code", CodeInvalid, $"Code '{definition.Code}' must be UPPER_SNAKE."));
                else if (!allowExistingCode && entries.ContainsKey(definition.Code))
                    issues.Add(new ValidationIssue($"{path}.code", CodeDuplicate, $"Code {definition.Code} is already defined."));

                if (definition.Role is null || !Roles.Contains(definition.Role))
                    issues.Add(new ValidationIssue($"{path}.role", RoleUnknown, $"Role '{definition.Role}' is unknown."));

                if (definition.UnitOut is null || !UnitCodes.Known.Contains(definition.UnitOut))
                    issues.Add(new ValidationIssue($"{path}.unitOut", UnitUnknown, $"Unit '{definition.UnitOut}' is unknown."));

                if (definition.Status is null || !DerivedStatuses.Contains(definition.Status))
                    issues.Add(new ValidationIssue($"{path}.status", StatusUnknown, "Status must be CANDIDATE or CONFIRMED."));
            }

            public IReadOnlyList<ParameterReference> CheckDifference(DerivedParameterDefinition definition)
            {
                var a = Resolve(definition.A, "a");
                var b = Resolve(definition.B, "b");

                if (a is null || b is null)
                    return References(a, b);

                if (a.Unit != b.Unit)
                    issues.Add(new ValidationIssue(path, UnitsDoNotReconcile, $"A difference needs one unit; a is {a.Unit} and b is {b.Unit}."));
                else if (definition.UnitOut != a.Unit)
                    issues.Add(new ValidationIssue($"{path}.unitOut", UnitsDoNotReconcile, $"The difference of two {a.Unit} values is {a.Unit}."));

                return References(a, b);
            }

            public IReadOnlyList<ParameterReference> CheckRatio(DerivedParameterDefinition definition)
            {
                var numerator = Resolve(definition.Numerator, "numerator");
                var denominator = Resolve(definition.Denominator, "denominator");

                if (definition.GuardZero is null)
                    issues.Add(new ValidationIssue($"{path}.guardZero", FieldRequired, "guardZero is required."));

                var sameUnit = numerator is not null && denominator is not null && numerator.Unit == denominator.Unit;

                if (sameUnit && definition.UnitOut != UnitCodes.Ratio)
                    issues.Add(new ValidationIssue($"{path}.unitOut", UnitsDoNotReconcile, "A ratio of two values in the same unit is RATIO."));

                return References(numerator, denominator);
            }

            public IReadOnlyList<ParameterReference> CheckLinearCombination(DerivedParameterDefinition definition)
            {
                if (definition.Offset is null)
                    issues.Add(new ValidationIssue($"{path}.offset", FieldRequired, "offset is required; use 0 for none."));

                if (definition.Terms is null || definition.Terms.Count == 0)
                {
                    issues.Add(new ValidationIssue($"{path}.terms", FieldRequired, "At least one term is required."));
                    return [];
                }

                var resolved = new List<ResolvedReference?>();

                for (var index = 0; index < definition.Terms.Count; index++)
                {
                    var term = definition.Terms[index];

                    if (term.Coefficient is null)
                        issues.Add(new ValidationIssue($"{path}.terms[{index}].coefficient", FieldRequired, "coefficient is required."));

                    resolved.Add(Resolve(term.Ref, $"terms[{index}].ref"));
                }

                var units = resolved.Where(reference => reference is not null).Select(reference => reference!.Unit).Distinct().ToList();

                if (units.Count > 1)
                    issues.Add(new ValidationIssue($"{path}.terms", UnitsDoNotReconcile, $"All terms need one unit; found {string.Join(", ", units)}."));

                return References(resolved.ToArray());
            }

            public IReadOnlyList<ParameterReference> CheckPowerRatio(DerivedParameterDefinition definition)
            {
                var reference = Resolve(definition.Ref, "ref");

                if (definition.ReferenceConstant is not { } constant || constant <= 0)
                    issues.Add(new ValidationIssue($"{path}.referenceConstant", ConstantInvalid, "referenceConstant must be a positive number."));

                if (definition.ExponentConstantRef is not null)
                    issues.Add(new ValidationIssue($"{path}.exponentConstantRef", ExponentRequiresCorrectionModel, "An exponent comes only from a CORRECTION_MODEL (P08)."));

                if (definition.UnitOut != UnitCodes.Ratio)
                    issues.Add(new ValidationIssue($"{path}.unitOut", UnitsDoNotReconcile, "A power ratio is RATIO."));

                return References(reference);
            }

            private ResolvedReference? Resolve(string? text, string field)
            {
                var fieldPath = $"{path}.{field}";

                if (text is null)
                {
                    issues.Add(new ValidationIssue(fieldPath, FieldRequired, $"{field} is required."));
                    return null;
                }

                var reference = ParameterReference.Parse(text);

                if (reference is null)
                {
                    issues.Add(new ValidationIssue(fieldPath, RefInvalid, $"'{text}' is not CODE, CODE[1], CODE[2] or CODE[pos]."));
                    return null;
                }

                if (!entries.TryGetValue(reference.Code, out var entry))
                {
                    issues.Add(new ValidationIssue(fieldPath, RefUnknown, $"{reference.Code} is not defined before this definition."));
                    return null;
                }

                if (!PositionFits(reference, entry))
                {
                    issues.Add(new ValidationIssue(fieldPath, RefPositionInvalid, $"'{text}' does not match the positions of {entry.Code}."));
                    return null;
                }

                if (!entry.IsNumeric)
                {
                    issues.Add(new ValidationIssue(fieldPath, RefNotNumeric, $"{entry.Code} is not numeric."));
                    return null;
                }

                return new ResolvedReference(reference, entry.Unit);
            }

            private static bool PositionFits(ParameterReference reference, CatalogEntry entry)
            {
                if (reference.UsesPositionPlaceholder)
                    return entry.Positions.Count > 0;

                if (reference.Position is { } position)
                    return entry.Positions.Contains(position);

                return entry.Positions.Count == 0;
            }

            private static IReadOnlyList<ParameterReference> References(params ResolvedReference?[] resolved)
                => resolved.Where(reference => reference is not null).Select(reference => reference!.Reference).ToList();
        }

        private sealed record ResolvedReference(ParameterReference Reference, string? Unit);
    }
}
