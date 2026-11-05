using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum BulkReviewOutcome
    {
        [Display(Name = "Applied")]
        APPLIED = 1,

        [Display(Name = "Refused")]
        REFUSED = 2,

        [Display(Name = "Flight Not Found")]
        NOT_FOUND = 3
    }
}
