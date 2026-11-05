using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum TransitionPick
    {
        [Display(Name = "First")]
        FIRST = 1,

        [Display(Name = "Last")]
        LAST = 2
    }
}
