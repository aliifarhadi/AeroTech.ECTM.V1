using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum BaselineValueOrigin
    {
        [Display(Name = "Suggested From File")]
        FILE_SUGGESTED = 1,

        [Display(Name = "Edited From File Suggestion")]
        FILE_EDITED = 2,

        [Display(Name = "Entered Manually")]
        MANUAL = 3
    }
}
