using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum RecomputeOutcome
    {
        [Display(Name = "Queued")]
        QUEUED = 1,

        [Display(Name = "Unchanged - Same Inputs")]
        UNCHANGED = 2,

        [Display(Name = "A Recompute Is Already Pending")]
        ALREADY_PENDING = 3
    }
}
