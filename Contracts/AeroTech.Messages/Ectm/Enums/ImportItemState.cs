using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ImportItemState
    {
        [Display(Name = "Previewed")]
        PREVIEWED = 1,

        [Display(Name = "Committed")]
        COMMITTED = 2,

        [Display(Name = "Skipped")]
        SKIPPED = 3,

        [Display(Name = "Failed")]
        FAILED = 4
    }
}
