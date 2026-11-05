using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ReviewState
    {
        [Display(Name = "Pending")]
        PENDING = 1,

        [Display(Name = "Accepted")]
        ACCEPTED = 2,

        [Display(Name = "Excluded")]
        EXCLUDED = 3,

        [Display(Name = "Superseded")]
        SUPERSEDED = 4
    }
}
