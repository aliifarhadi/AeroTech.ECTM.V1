using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum TimeTrust
    {
        [Display(Name = "Trusted")]
        TRUSTED = 1,

        [Display(Name = "Untrusted")]
        UNTRUSTED = 2
    }
}
