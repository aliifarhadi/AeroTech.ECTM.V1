using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum BaselineState
    {
        [Display(Name = "Draft")]
        DRAFT = 1,

        [Display(Name = "Published")]
        PUBLISHED = 2,

        [Display(Name = "Retired")]
        RETIRED = 3
    }
}
