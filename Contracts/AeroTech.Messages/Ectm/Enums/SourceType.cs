using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum SourceType
    {
        [Display(Name = "Real Source")]
        REAL_SOURCE = 1,

        [Display(Name = "Synthetic Test Only")]
        SYNTHETIC_TEST_ONLY = 2
    }
}
