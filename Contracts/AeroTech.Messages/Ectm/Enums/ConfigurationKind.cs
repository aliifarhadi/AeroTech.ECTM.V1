using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ConfigurationKind
    {
        [Display(Name = "Data Profile")]
        DATA_PROFILE = 1,

        [Display(Name = "Parameter Catalog")]
        PARAMETER_CATALOG = 2,

        [Display(Name = "Quality Rules")]
        QUALITY_RULES = 3,

        [Display(Name = "Phase Rule Set")]
        PHASE_RULESET = 4,

        [Display(Name = "Snapshot Rules")]
        SNAPSHOT_RULES = 5,

        [Display(Name = "Baseline Rules")]
        BASELINE_RULES = 6,

        [Display(Name = "Correction Model")]
        CORRECTION_MODEL = 7,

        [Display(Name = "Analytics Rules")]
        ANALYTICS_RULES = 8,

        [Display(Name = "Report Definition")]
        REPORT_DEFINITION = 9
    }
}
