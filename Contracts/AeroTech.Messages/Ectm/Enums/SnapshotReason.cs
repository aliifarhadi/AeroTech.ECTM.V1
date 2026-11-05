using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum SnapshotReason
    {
        [Display(Name = "No Phase Result")]
        NO_PHASE_RESULT = 1,

        [Display(Name = "No Lift-off Anchor and No High N1")]
        NO_LIFTOFF_ANCHOR_AND_NO_HIGH_N1 = 2,

        [Display(Name = "No N1 in the Search Window")]
        NO_N1 = 3,

        [Display(Name = "Too Few N1 Samples in the Window")]
        INSUFFICIENT_WINDOW = 4,

        [Display(Name = "No EGT in the Selected Window")]
        NO_EGT_IN_SELECTED_WINDOW = 5,

        [Display(Name = "No EGT in the Search Window")]
        NO_EGT_IN_SEARCH_WINDOW = 6,

        [Display(Name = "No Eligible Stable Block")]
        NO_ELIGIBLE_BLOCK = 7,

        [Display(Name = "Report Window Outside the Block")]
        REPORT_WINDOW_OUTSIDE_BLOCK = 8,

        [Display(Name = "An Engine Has No Stable Block")]
        MISSING_ENGINE_WINDOW = 9,

        [Display(Name = "Joint Block Shorter than the Report Window")]
        INSUFFICIENT_JOINT_BLOCK = 10,

        [Display(Name = "Too Few Samples with Both EGT Values")]
        INSUFFICIENT_SAME_INDEX_OVERLAP = 11
    }
}
