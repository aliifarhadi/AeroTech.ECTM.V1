using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ReferenceMatch
    {
        [Display(Name = "Matches the Pack Reference")]
        MATCHES_REFERENCE = 1,

        [Display(Name = "Modified from the Pack Reference")]
        MODIFIED_FROM_REFERENCE = 2
    }
}
