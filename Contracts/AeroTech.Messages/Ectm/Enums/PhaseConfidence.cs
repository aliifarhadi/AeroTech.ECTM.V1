using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum PhaseConfidence
    {
        [Display(Name = "High")]
        HIGH = 1,

        [Display(Name = "Medium")]
        MEDIUM = 2,

        [Display(Name = "Low")]
        LOW = 3
    }
}
