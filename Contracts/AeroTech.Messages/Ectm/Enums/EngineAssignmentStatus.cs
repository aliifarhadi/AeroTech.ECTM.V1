using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum EngineAssignmentStatus
    {
        [Display(Name = "Engine")]
        ENGINE = 1,

        [Display(Name = "Position Only")]
        POSITION_ONLY = 2,

        [Display(Name = "Needs Confirmation")]
        NEEDS_CONFIRMATION = 3
    }
}
