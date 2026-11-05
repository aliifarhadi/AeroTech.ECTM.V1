using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ProcessingJobKind
    {
        [Display(Name = "Import Commit")]
        IMPORT_COMMIT = 1,

        [Display(Name = "Recompute")]
        RECOMPUTE = 2
    }
}
