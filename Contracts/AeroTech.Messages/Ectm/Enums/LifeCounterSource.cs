using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum LifeCounterSource
    {
        [Display(Name = "Manual")]
        MANUAL = 1,

        [Display(Name = "Computed")]
        COMPUTED = 2
    }
}
