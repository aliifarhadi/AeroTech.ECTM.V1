using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ComparisonStatus
    {
        [Display(Name = "Descriptive Raw Difference")]
        DESCRIPTIVE_RAW = 1,

        [Display(Name = "Descriptive Raw Difference, Unit Unverified")]
        DESCRIPTIVE_RAW_UNIT_UNVERIFIED = 2,

        [Display(Name = "Not Comparable")]
        NOT_COMPARABLE = 3,

        [Display(Name = "Flight Not Final")]
        NOT_FINAL = 4,

        [Display(Name = "Stale Analysis Revision")]
        STALE_REVISION = 5
    }
}
