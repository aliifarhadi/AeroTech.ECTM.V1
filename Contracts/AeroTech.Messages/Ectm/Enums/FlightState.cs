using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum FlightState
    {
        [Display(Name = "Needs Identity")]
        NEEDS_IDENTITY = 1,

        [Display(Name = "Pending Review")]
        PENDING_REVIEW = 2,

        [Display(Name = "Held")]
        HELD = 3,

        [Display(Name = "Accepted")]
        ACCEPTED = 4,

        [Display(Name = "Excluded")]
        EXCLUDED = 5
    }
}
