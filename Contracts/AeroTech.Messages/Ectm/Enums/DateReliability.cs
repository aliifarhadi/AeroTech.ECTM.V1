using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum DateReliability
    {
        [Display(Name = "Exact")]
        EXACT = 1,

        [Display(Name = "Day Only")]
        DAY_ONLY = 2,

        [Display(Name = "Approximate")]
        APPROXIMATE = 3
    }
}
