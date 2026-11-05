using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ProcessingJobState
    {
        [Display(Name = "Queued")]
        QUEUED = 1,

        [Display(Name = "Running")]
        RUNNING = 2,

        [Display(Name = "Completed")]
        COMPLETED = 3,

        [Display(Name = "Failed")]
        FAILED = 4
    }
}
