using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ImportItemOutcome
    {
        [Display(Name = "New")]
        NEW = 1,

        [Display(Name = "Duplicate")]
        DUPLICATE = 2,

        [Display(Name = "Rejected")]
        REJECTED = 3
    }
}
