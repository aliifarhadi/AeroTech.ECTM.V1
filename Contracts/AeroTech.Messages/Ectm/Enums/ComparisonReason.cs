using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ComparisonReason
    {
        [Display(Name = "Different Physical Engine")]
        DIFFERENT_ESN = 1,

        [Display(Name = "Snapshot Kind Mismatch")]
        SNAPSHOT_KIND_MISMATCH = 2,

        [Display(Name = "Selector Mismatch")]
        SELECTOR_MISMATCH = 3,

        [Display(Name = "Parameter Mismatch")]
        PARAMETER_MISMATCH = 4,

        [Display(Name = "Unit Mismatch")]
        UNIT_MISMATCH = 5,

        [Display(Name = "Missing Observation")]
        MISSING_OBSERVATION = 6,

        [Display(Name = "Environmental Effects Not Removed")]
        ENVIRONMENT_NOT_REMOVED = 7,

        [Display(Name = "Context Unknown")]
        CONTEXT_UNKNOWN = 8,

        [Display(Name = "Context Unverified: Channel Coherence")]
        CONTEXT_UNVERIFIED_CHANNEL_COHERENCE = 9,

        [Display(Name = "Condition Outside Tolerance")]
        CONDITION_OUT_OF_TOLERANCE = 10,

        [Display(Name = "Configuration Differs")]
        CONFIGURATION_DIFFERS = 11,

        [Display(Name = "Snapshot Rule Set Changed")]
        SNAPSHOT_RULESET_CHANGED = 12,

        [Display(Name = "Reference Source Point")]
        REFERENCE_SOURCE_POINT = 13,

        [Display(Name = "Retrospective")]
        RETROSPECTIVE = 14,

        [Display(Name = "Temporal Order Unverified")]
        TEMPORAL_ORDER_UNVERIFIED = 15,

        [Display(Name = "No Approved Correction Model")]
        NO_APPROVED_CORRECTION_MODEL = 16
    }
}
