using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum AnchorBasis
    {
        [Display(Name = "Radio Height Crossing")]
        RADIO_HEIGHT_CROSSING = 1,

        [Display(Name = "Air/Ground Discrete (Lagged)")]
        AIRGROUND_DISCRETE_LAGGED = 2,

        [Display(Name = "Partial Start")]
        PARTIAL_START = 3,

        [Display(Name = "Partial End")]
        PARTIAL_END = 4,

        [Display(Name = "Not Found")]
        NOT_FOUND = 5
    }
}
