using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum EventEvaluability
    {
        [Display(Name = "Evaluated")]
        EVALUATED = 1,

        [Display(Name = "Unverified Enumeration")]
        UNVERIFIED_ENUM = 2,

        [Display(Name = "Missing Channel")]
        MISSING = 3
    }
}
