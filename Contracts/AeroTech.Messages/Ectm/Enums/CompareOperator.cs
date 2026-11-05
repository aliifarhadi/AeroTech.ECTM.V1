using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum CompareOperator
    {
        [Display(Name = "Less Than")]
        LT = 1,

        [Display(Name = "At Most")]
        LE = 2,

        [Display(Name = "Greater Than")]
        GT = 3,

        [Display(Name = "At Least")]
        GE = 4
    }
}
