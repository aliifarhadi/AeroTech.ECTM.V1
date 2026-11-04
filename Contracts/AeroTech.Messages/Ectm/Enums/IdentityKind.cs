using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum IdentityKind
    {
        [Display(Name = "Engine")]
        ENGINE = 1,

        [Display(Name = "Position Only")]
        POSITION_ONLY = 2
    }
}
