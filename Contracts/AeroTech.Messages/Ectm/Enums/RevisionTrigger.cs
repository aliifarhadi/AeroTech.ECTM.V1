using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum RevisionTrigger
    {
        [Display(Name = "Import")]
        IMPORT = 1,

        [Display(Name = "Recompute")]
        RECOMPUTE = 2,

        [Display(Name = "Identity Change")]
        IDENTITY_CHANGE = 3
    }
}
