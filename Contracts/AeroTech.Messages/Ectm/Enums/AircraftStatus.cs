using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum AircraftStatus
    {
        [Display(Name = "Active")]
        ACTIVE = 1,

        [Display(Name = "Stored")]
        STORED = 2,

        [Display(Name = "Retired")]
        RETIRED = 3
    }
}
