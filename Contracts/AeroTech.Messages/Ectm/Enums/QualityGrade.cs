using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum QualityGrade
    {
        [Display(Name = "Grade A")]
        A = 1,

        [Display(Name = "Grade B")]
        B = 2,

        [Display(Name = "Grade C")]
        C = 3,

        [Display(Name = "Not Usable")]
        NOT_USABLE = 4
    }
}
