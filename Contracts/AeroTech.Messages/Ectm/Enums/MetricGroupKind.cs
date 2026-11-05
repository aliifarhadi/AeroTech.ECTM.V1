using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum MetricGroupKind
    {
        [Display(Name = "Whole Flight")]
        WHOLE_FLIGHT = 1,

        [Display(Name = "Phase")]
        PHASE = 2,

        [Display(Name = "Segment")]
        SEGMENT = 3,

        [Display(Name = "Steady Cruise Block")]
        STEADY_CRUISE = 4
    }
}
