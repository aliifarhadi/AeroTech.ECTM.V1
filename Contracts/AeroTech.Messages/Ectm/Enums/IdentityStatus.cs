using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum IdentityStatus
    {
        [Display(Name = "Resolved")]
        RESOLVED = 1,

        [Display(Name = "Needs Confirmation")]
        NEEDS_CONFIRMATION = 2,

        [Display(Name = "Unresolved")]
        UNRESOLVED = 3
    }
}
