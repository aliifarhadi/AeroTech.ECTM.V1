using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum TakeoffSearchBasis
    {
        [Display(Name = "Anchored on Lift-off")]
        LIFTOFF_ANCHORED = 1,

        [Display(Name = "Partial Start - First High N1")]
        PARTIAL_START_FIRST_HIGH_N1 = 2
    }
}
