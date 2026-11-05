using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum RevisionStatus
    {
        [Display(Name = "Pending")]
        PENDING = 1,

        [Display(Name = "Current")]
        CURRENT = 2,

        [Display(Name = "Superseded")]
        SUPERSEDED = 3,

        [Display(Name = "Failed")]
        FAILED = 4
    }
}
