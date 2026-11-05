using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ReviewAction
    {
        [Display(Name = "Accept")]
        ACCEPT = 1,

        [Display(Name = "Exclude")]
        EXCLUDE = 2,

        [Display(Name = "Hold")]
        HOLD = 3,

        [Display(Name = "Release")]
        RELEASE = 4,

        [Display(Name = "Request Identity")]
        REQUEST_IDENTITY = 5
    }
}
