using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum EngineStatus
    {
        [Display(Name = "Active")]
        ACTIVE = 1,

        [Display(Name = "Stored")]
        STORED = 2,

        [Display(Name = "Leased")]
        LEASED = 3,

        [Display(Name = "Retired")]
        RETIRED = 4
    }
}
