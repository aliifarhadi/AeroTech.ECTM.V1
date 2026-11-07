using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum BaselineInputProvenance
    {
        [Display(Name = "Manual")]
        MANUAL = 1,

        [Display(Name = "File Prefilled")]
        FILE_PREFILLED = 2,

        [Display(Name = "Mixed")]
        MIXED = 3
    }
}
