using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed class RuleDocumentCheck(ValidatedCatalog? catalog, List<ValidationIssue> issues)
    {
        public const string FieldRequired = "FIELD_REQUIRED";
        public const string ValueInvalid = "VALUE_INVALID";
        public const string ChannelUnknown = "CHANNEL_UNKNOWN";
        public const string ChannelKindInvalid = "CHANNEL_KIND_INVALID";
        public const string ReferenceUnknown = "REFERENCE_UNKNOWN";
        public const string CodeDuplicate = "CODE_DUPLICATE";

        private readonly HashSet<string> _derivedChannels = new(StringComparer.Ordinal);
        private readonly HashSet<string> _anchors = new(StringComparer.Ordinal);
        private readonly HashSet<string> _conditions = new(StringComparer.Ordinal);
        private readonly HashSet<FlightPhaseCode> _labels = [];
        private readonly HashSet<string> _parameters = new(StringComparer.Ordinal);

        public bool ConditionsAvailable { get; set; }

        public IReadOnlySet<FlightPhaseCode> Labels => _labels;

        public IReadOnlySet<string> Anchors => _anchors;

        public IReadOnlySet<string> Conditions => _conditions;

        public IReadOnlySet<string> DerivedChannels => _derivedChannels;

        public IReadOnlySet<string> Parameters => _parameters;

        public void Issue(string path, string code, string message) => issues.Add(new ValidationIssue(path, code, message));

        public void Required(object? value, string path)
        {
            if (value is null)
                Issue(path, FieldRequired, "A value is required.");
        }

        public void Text(string? value, string path)
        {
            if (string.IsNullOrWhiteSpace(value))
                Issue(path, FieldRequired, "A value is required.");
        }

        public void Positive(double? value, string path)
        {
            if (value is not > 0)
                Issue(path, ValueInvalid, "A positive number is required.");
        }

        public void NotNegative(double? value, string path)
        {
            if (value is not >= 0)
                Issue(path, ValueInvalid, "A number of zero or more is required.");
        }

        public void AtLeast(int? value, int minimum, string path)
        {
            if (value is null || value < minimum)
                Issue(path, ValueInvalid, $"A whole number of at least {minimum} is required.");
        }

        public void Entries<TItem>(IReadOnlyList<TItem>? items, string path, bool allowEmpty)
        {
            if (items is null)
                Issue(path, FieldRequired, "A list is required.");
            else if (!allowEmpty && items.Count == 0)
                Issue(path, ValueInvalid, "The list needs at least one entry.");
        }

        public void Label(FlightPhaseCode? label, PhaseUnknownReason? unknownReason, string path)
        {
            if (label is not { } present)
            {
                Issue($"{path}.label", FieldRequired, "A phase label is required.");
                return;
            }

            _labels.Add(present);

            if (present == FlightPhaseCode.UNKNOWN && unknownReason is null)
                Issue($"{path}.unknownReason", FieldRequired, "An UNKNOWN label needs a reason.");

            if (present != FlightPhaseCode.UNKNOWN && unknownReason is not null)
                Issue($"{path}.unknownReason", ValueInvalid, "Only an UNKNOWN label carries a reason.");
        }

        public void ProducesLabel(FlightPhaseCode label) => _labels.Add(label);

        public void DefineDerivedChannel(string? code, string path)
        {
            if (!Code(code, path))
                return;

            if (catalog?.Find(code!) is not null)
                Issue(path, CodeDuplicate, $"{code} is already a parameter of the catalog.");

            if (!_derivedChannels.Add(code!))
                Issue(path, CodeDuplicate, $"{code} is defined twice.");
        }

        public void DefineAnchor(string? code, string path)
        {
            if (Code(code, path) && !_anchors.Add(code!))
                Issue(path, CodeDuplicate, $"{code} is defined twice.");
        }

        public void DefineCondition(string? code, string path)
        {
            if (Code(code, path) && !_conditions.Add(code!))
                Issue(path, CodeDuplicate, $"{code} is defined twice.");
        }

        public void Anchor(string? code, string path)
        {
            if (Code(code, path) && !_anchors.Contains(code!))
                Issue(path, ReferenceUnknown, $"{code} is not an anchor defined above this point.");
        }

        public void Condition(string? code, string path)
        {
            if (!Code(code, path))
                return;

            if (!ConditionsAvailable)
                Issue(path, ReferenceUnknown, "Conditions are evaluated after the anchors and cannot be used here.");
            else if (!_conditions.Contains(code!))
                Issue(path, ReferenceUnknown, $"{code} is not a condition defined above this point.");
        }

        public void Mask(string? code, string path)
        {
            if (Code(code, path) && !QualitySpanCodes.All.Contains(code!))
                Issue(path, ReferenceUnknown, $"'{code}' is not a quality span code.");
        }

        public void NumericChannel(string? key, string path)
        {
            if (!Code(key, path) || _derivedChannels.Contains(key!))
                return;

            if (RecordedEntryOf(key!, path) is { } entry && !entry.IsNumeric)
                Issue(path, ChannelKindInvalid, $"{key} must be numeric.");
        }

        public void TextChannel(string? key, string path)
        {
            if (!Code(key, path))
                return;

            if (_derivedChannels.Contains(key!))
            {
                Issue(path, ChannelKindInvalid, $"{key} is a numeric derived channel.");
                return;
            }

            if (RecordedEntryOf(key!, path) is { } entry && entry.IsNumeric)
                Issue(path, ChannelKindInvalid, $"{key} must be a text or enumerated parameter.");
        }

        public void Literal(string? key, string? value, string path)
        {
            if (string.IsNullOrEmpty(value))
            {
                Issue(path, FieldRequired, "A literal value is required.");
                return;
            }

            if (ParameterReference.Parse(key) is not { } reference || catalog?.Find(reference.Code) is not { } entry)
                return;

            if (entry.EnumValues is { Count: > 0 } values && !values.Contains(value))
                Issue(path, ValueInvalid, $"'{value}' is not one of the recorded values of {reference.Code}: {string.Join(", ", values)}.");
        }

        public void Percent(double? value, string path)
        {
            if (value is not (> 0 and <= 100))
                Issue(path, ValueInvalid, "A percentage above 0 and up to 100 is required.");
        }

        public void EngineParameter(string? code, string path)
        {
            if (ParameterOf(code, path) is not { } entry)
                return;

            if (!entry.IsNumeric || entry.Positions.Count == 0)
                Issue(path, ChannelKindInvalid, $"{code} must be a numeric engine parameter with positions.");
        }

        public CatalogEntry? ParameterOf(string? code, string path)
        {
            if (!Code(code, path))
                return null;

            _parameters.Add(code!);

            if (catalog is null)
                return null;

            var entry = catalog.Find(code!);

            if (entry is null || entry.IsDerived)
                Issue(path, ChannelUnknown, $"{code} is not a recorded parameter of the active catalog.");

            return entry is { IsDerived: false } ? entry : null;
        }

        private CatalogEntry? RecordedEntryOf(string key, string path)
        {
            var reference = ParameterReference.Parse(key);

            if (reference is null || reference.UsesPositionPlaceholder)
            {
                Issue(path, ValueInvalid, $"'{key}' must be CODE or CODE[1|2].");
                return null;
            }

            if (ParameterOf(reference.Code, path) is not { } entry)
                return null;

            var positionMatches = reference.Position is { } position ? entry.Positions.Contains(position) : entry.Positions.Count == 0;

            if (!positionMatches)
                Issue(path, ValueInvalid, $"'{key}' does not match the positions of {reference.Code}.");

            return entry;
        }

        private bool Code(string? code, string path)
        {
            if (!string.IsNullOrWhiteSpace(code))
                return true;

            Issue(path, FieldRequired, "A code is required.");
            return false;
        }
    }
}
