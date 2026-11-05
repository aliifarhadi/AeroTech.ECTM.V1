using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum SnapshotAvailability
    {
        [Display(Name = "Available")]
        AVAILABLE = 1,

        [Display(Name = "Not Evaluable")]
        NOT_EVALUABLE = 2
    }
}
