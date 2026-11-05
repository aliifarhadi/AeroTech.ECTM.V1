using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ValidationSeverity
    {
        [Display(Name = "Blocking")]
        BLOCKING = 1,

        [Display(Name = "Warning")]
        WARNING = 2,

        [Display(Name = "Info")]
        INFO = 3
    }
}
