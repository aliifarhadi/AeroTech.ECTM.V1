using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum SnapshotKind
    {
        [Display(Name = "Takeoff N1 Peak Context")]
        TAKEOFF_N1_PEAK_CONTEXT = 1,

        [Display(Name = "Takeoff EGT Peak Observed")]
        TAKEOFF_EGT_PEAK_OBSERVED = 2,

        [Display(Name = "Cruise")]
        CRUISE = 3
    }
}
