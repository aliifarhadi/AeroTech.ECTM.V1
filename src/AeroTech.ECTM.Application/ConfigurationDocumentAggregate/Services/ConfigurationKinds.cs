using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public static class ConfigurationKinds
    {
        public static readonly IReadOnlyDictionary<ConfigurationKind, string> DeliveryPhase = new Dictionary<ConfigurationKind, string>
        {
            [ConfigurationKind.DATA_PROFILE] = "P02",
            [ConfigurationKind.PARAMETER_CATALOG] = "P02",
            [ConfigurationKind.QUALITY_RULES] = "P04",
            [ConfigurationKind.PHASE_RULESET] = "P05",
            [ConfigurationKind.SNAPSHOT_RULES] = "P06",
            [ConfigurationKind.BASELINE_RULES] = "P07",
            [ConfigurationKind.CORRECTION_MODEL] = "P08",
            [ConfigurationKind.REPORT_DEFINITION] = "P11",
            [ConfigurationKind.ANALYTICS_RULES] = "P14"
        };

        public static readonly IReadOnlyList<ConfigurationKind> RequiredForParser =
        [
            ConfigurationKind.PARAMETER_CATALOG,
            ConfigurationKind.DATA_PROFILE
        ];

        public static ConfigurationKind Parse(string? code)
        {
            foreach (var kind in Enum.GetValues<ConfigurationKind>())
            {
                if (kind.ToString() == code)
                    return kind;
            }

            throw ExceptionFactory.ConfigurationKindNotFound(code);
        }
    }
}
