using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ImportBatchState
    {
        [Display(Name = "Previewed")]
        PREVIEWED = 1,

        [Display(Name = "Committing")]
        COMMITTING = 2,

        [Display(Name = "Committed")]
        COMMITTED = 3,

        [Display(Name = "Failed")]
        FAILED = 4
    }
}
