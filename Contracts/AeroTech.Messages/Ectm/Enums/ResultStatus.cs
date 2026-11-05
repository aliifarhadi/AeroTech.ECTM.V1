using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ResultStatus
    {
        [Display(Name = "Provisional")]
        PROVISIONAL = 1,

        [Display(Name = "Final")]
        FINAL = 2,

        [Display(Name = "Superseded")]
        SUPERSEDED = 3
    }
}
