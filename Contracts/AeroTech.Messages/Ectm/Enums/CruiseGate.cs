using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum CruiseGate
    {
        [Display(Name = "Window Crosses a Gap")]
        NOT_CONTIGUOUS = 1,

        [Display(Name = "Not Cruise")]
        NOT_CRUISE = 2,

        [Display(Name = "Inside a Quality Mask")]
        QUALITY_MASK = 3,

        [Display(Name = "N1 Missing")]
        N1_MISSING = 4,

        [Display(Name = "N1 Not Stable")]
        N1_RANGE = 5,

        [Display(Name = "Altitude or Mach Missing")]
        ALTITUDE_OR_MACH_MISSING = 6,

        [Display(Name = "Altitude Not Stable")]
        ALTITUDE_RANGE = 7,

        [Display(Name = "Mach Not Stable")]
        MACH_RANGE = 8,

        [Display(Name = "Configuration Changed")]
        CONFIGURATION_CHANGED = 9
    }
}
