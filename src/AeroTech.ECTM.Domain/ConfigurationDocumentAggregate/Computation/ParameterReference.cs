using System.Text.RegularExpressions;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record ParameterReference(string Code, int? Position, bool UsesPositionPlaceholder)
    {
        private const string PositionPlaceholder = "pos";

        private static readonly Regex Pattern = new(@"^(?<code>[A-Z][A-Z0-9_]*)(\[(?<index>1|2|pos)\])?$", RegexOptions.Compiled);

        public static ParameterReference? Parse(string? text)
        {
            if (text is null)
                return null;

            var match = Pattern.Match(text);

            if (!match.Success)
                return null;

            var code = match.Groups["code"].Value;
            var index = match.Groups["index"];

            if (!index.Success)
                return new ParameterReference(code, null, false);

            if (index.Value == PositionPlaceholder)
                return new ParameterReference(code, null, true);

            return new ParameterReference(code, int.Parse(index.Value), false);
        }

        public string Resolve(int? position)
        {
            if (UsesPositionPlaceholder)
                return $"{Code}[{position}]";

            if (Position is { } fixedPosition)
                return $"{Code}[{fixedPosition}]";

            return Code;
        }
    }
}
