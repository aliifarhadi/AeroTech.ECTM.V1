namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class DataProfileValidator
    {
        public const string ProfileCodeRequired = "PROFILE_CODE_REQUIRED";
        public const string FileTypeUnsupported = "FILE_TYPE_UNSUPPORTED";
        public const string DelimiterInvalid = "DELIMITER_INVALID";
        public const string EncodingUnsupported = "ENCODING_UNSUPPORTED";
        public const string ColumnCountMismatch = "COLUMN_COUNT_MISMATCH";
        public const string OrdinalOutOfRange = "ORDINAL_OUT_OF_RANGE";
        public const string OrdinalDuplicate = "ORDINAL_DUPLICATE";
        public const string OrdinalMissing = "ORDINAL_MISSING";
        public const string HeaderRequired = "HEADER_REQUIRED";
        public const string OccurrenceMismatch = "OCCURRENCE_MISMATCH";
        public const string ActiveCatalogMissing = "ACTIVE_CATALOG_MISSING";
        public const string ParameterUnknown = "PARAMETER_UNKNOWN";
        public const string ParameterIsDerived = "PARAMETER_IS_DERIVED";
        public const string PositionInvalid = "POSITION_INVALID";
        public const string ColumnCollision = "COLUMN_COLLISION";
        public const string UnitUnknown = "UNIT_UNKNOWN";
        public const string UnitMismatch = "UNIT_MISMATCH";
        public const string ConversionUnsupported = "CONVERSION_UNSUPPORTED";
        public const string MissingTokensRequired = "MISSING_TOKENS_REQUIRED";
        public const string TimeChannelMissing = "TIME_CHANNEL_MISSING";
        public const string RequiredChannelMissing = "REQUIRED_CHANNEL_MISSING";

        public const string CsvFileType = "CSV";
        public const string Utf8Encoding = "UTF-8";
        public const string IdentityConversion = "IDENTITY";

        public static readonly IReadOnlyList<string> RequiredForPublish =
        [
            "ELAPSED_S",
            "PRESSURE_ALT_FT",
            "CAS_KT",
            "MACH",
            "TAT_C",
            "GROUNDSPEED_KT",
            "RADIO_HEIGHT_FT",
            "N1_PCT[1]",
            "N1_PCT[2]",
            "N2_PCT[1]",
            "N2_PCT[2]",
            "EGT_C[1]",
            "EGT_C[2]",
            "FUEL_FLOW_PPH[1]",
            "FUEL_FLOW_PPH[2]"
        ];

        public static ProfileValidation Validate(string content, ValidatedCatalog? catalog, bool forPublish)
        {
            var issues = new List<ValidationIssue>();
            var profile = ContentJson.Read<DataProfileContent>(content, issues);

            if (profile is null)
                return new ProfileValidation(null, issues);

            CheckHeader(profile, issues);

            var columns = profile.Columns ?? [];
            CheckOrdinals(profile, columns, issues);
            CheckHeaderOccurrences(columns, issues);

            if (catalog is null)
            {
                issues.Add(new ValidationIssue("$", ActiveCatalogMissing, "Publish a PARAMETER_CATALOG first; the profile is checked against the active catalog."));
                return new ProfileValidation(profile, issues);
            }

            var mapped = CheckMappings(columns, catalog, issues);

            if (profile.TimeParameterCode is null || !mapped.ContainsKey(profile.TimeParameterCode))
                issues.Add(new ValidationIssue("$.timeParameterCode", TimeChannelMissing, $"The time channel '{profile.TimeParameterCode}' is not mapped to a column."));

            if (forPublish)
                CheckRequiredChannels(mapped, issues);

            return new ProfileValidation(profile, issues);
        }

        public static string KeyOf(string parameterCode, int? position)
        {
            if (position is { } enginePosition)
                return $"{parameterCode}[{enginePosition}]";

            return parameterCode;
        }

        private static void CheckHeader(DataProfileContent profile, List<ValidationIssue> issues)
        {
            if (string.IsNullOrWhiteSpace(profile.ProfileCode))
                issues.Add(new ValidationIssue("$.profileCode", ProfileCodeRequired, "A profile code is required."));

            if (profile.FileType != CsvFileType)
                issues.Add(new ValidationIssue("$.fileType", FileTypeUnsupported, $"File type '{profile.FileType}' is not supported; CSV is."));

            if (profile.Delimiter is not { Length: 1 })
                issues.Add(new ValidationIssue("$.delimiter", DelimiterInvalid, "The delimiter must be one character."));

            if (profile.Encoding != Utf8Encoding)
                issues.Add(new ValidationIssue("$.encoding", EncodingUnsupported, $"Encoding '{profile.Encoding}' is not supported; UTF-8 is."));
        }

        private static void CheckOrdinals(DataProfileContent profile, IReadOnlyList<DataProfileColumn> columns, List<ValidationIssue> issues)
        {
            var columnCount = profile.ColumnCount ?? 0;

            if (columnCount <= 0 || columnCount != columns.Count)
            {
                issues.Add(new ValidationIssue("$.columnCount", ColumnCountMismatch, $"columnCount {profile.ColumnCount} must equal the {columns.Count} listed columns."));
                return;
            }

            var seen = new HashSet<int>();

            for (var index = 0; index < columns.Count; index++)
            {
                var ordinal = columns[index].Ordinal;
                var path = $"$.columns[{index}].ordinal";

                if (ordinal is not { } value || value < 0 || value >= columnCount)
                {
                    issues.Add(new ValidationIssue(path, OrdinalOutOfRange, $"Ordinal {ordinal} is outside 0..{columnCount - 1}."));
                    continue;
                }

                if (!seen.Add(value))
                    issues.Add(new ValidationIssue(path, OrdinalDuplicate, $"Ordinal {value} is mapped twice."));
            }

            for (var ordinal = 0; ordinal < columnCount; ordinal++)
            {
                if (!seen.Contains(ordinal))
                    issues.Add(new ValidationIssue("$.columns", OrdinalMissing, $"Ordinal {ordinal} has no column."));
            }
        }

        private static void CheckHeaderOccurrences(IReadOnlyList<DataProfileColumn> columns, List<ValidationIssue> issues)
        {
            var occurrences = new Dictionary<string, int>(StringComparer.Ordinal);
            var inOrdinalOrder = columns
                .Select((column, index) => (column, index))
                .Where(item => item.column.Ordinal is not null)
                .OrderBy(item => item.column.Ordinal);

            foreach (var (column, index) in inOrdinalOrder)
            {
                var path = $"$.columns[{index}]";

                if (string.IsNullOrEmpty(column.ExactHeader))
                {
                    issues.Add(new ValidationIssue($"{path}.exactHeader", HeaderRequired, "The exact header text is required."));
                    continue;
                }

                var trimmedHeader = column.ExactHeader.Trim();
                var occurrence = occurrences.GetValueOrDefault(trimmedHeader) + 1;
                occurrences[trimmedHeader] = occurrence;

                if (column.HeaderOccurrence != occurrence)
                    issues.Add(new ValidationIssue($"{path}.headerOccurrence", OccurrenceMismatch, $"Header '{trimmedHeader}' at ordinal {column.Ordinal} is occurrence {occurrence}, not {column.HeaderOccurrence}."));
            }
        }

        private static Dictionary<string, int> CheckMappings(IReadOnlyList<DataProfileColumn> columns, ValidatedCatalog catalog, List<ValidationIssue> issues)
        {
            var mapped = new Dictionary<string, int>(StringComparer.Ordinal);

            for (var index = 0; index < columns.Count; index++)
            {
                var column = columns[index];
                var path = $"$.columns[{index}]";

                if (column.MissingTokens is null)
                    issues.Add(new ValidationIssue($"{path}.missingTokens", MissingTokensRequired, "missingTokens is required; use [] for none."));

                if (column.SourceUnit is null || !UnitCodes.Known.Contains(column.SourceUnit))
                    issues.Add(new ValidationIssue($"{path}.sourceUnit", UnitUnknown, $"Unit '{column.SourceUnit}' is unknown."));

                if (column.ParameterCode is null)
                    continue;

                var entry = catalog.Find(column.ParameterCode);

                if (entry is null)
                {
                    issues.Add(new ValidationIssue($"{path}.parameterCode", ParameterUnknown, $"{column.ParameterCode} is not in the active catalog."));
                    continue;
                }

                if (entry.IsDerived)
                {
                    issues.Add(new ValidationIssue($"{path}.parameterCode", ParameterIsDerived, $"{column.ParameterCode} is derived; a recorder column maps to a source parameter."));
                    continue;
                }

                if (!PositionFits(column.Position, entry))
                {
                    issues.Add(new ValidationIssue($"{path}.position", PositionInvalid, $"Position {column.Position} does not fit {entry.Code} (positions: [{string.Join(", ", entry.Positions)}])."));
                    continue;
                }

                CheckUnit(column, entry, path, issues);

                var key = KeyOf(entry.Code, column.Position);

                if (mapped.TryGetValue(key, out var firstOrdinal))
                {
                    issues.Add(new ValidationIssue($"{path}.parameterCode", ColumnCollision, $"{key} is already mapped by ordinal {firstOrdinal}."));
                    continue;
                }

                mapped[key] = column.Ordinal ?? -1;
            }

            return mapped;
        }

        private static bool PositionFits(int? position, CatalogEntry entry)
        {
            if (position is { } enginePosition)
                return entry.Positions.Contains(enginePosition);

            return entry.Positions.Count == 0;
        }

        private static void CheckUnit(DataProfileColumn column, CatalogEntry entry, string path, List<ValidationIssue> issues)
        {
            if (column.Conversion != IdentityConversion)
            {
                issues.Add(new ValidationIssue($"{path}.conversion", ConversionUnsupported, $"Conversion '{column.Conversion}' is not available; only IDENTITY is."));
                return;
            }

            if (column.SourceUnit != entry.Unit)
                issues.Add(new ValidationIssue($"{path}.sourceUnit", UnitMismatch, $"{entry.Code} is in {entry.Unit}; the column says {column.SourceUnit} with no conversion."));
        }

        private static void CheckRequiredChannels(Dictionary<string, int> mapped, List<ValidationIssue> issues)
        {
            foreach (var required in RequiredForPublish)
            {
                if (!mapped.ContainsKey(required))
                    issues.Add(new ValidationIssue("$.columns", RequiredChannelMissing, $"{required} must be mapped before the profile can be published."));
            }
        }
    }
}
