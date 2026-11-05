using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum PhaseUnknownReason
    {
        [Display(Name = "Altitude Rate Untrusted")]
        ALTITUDE_RATE_UNTRUSTED = 1,

        [Display(Name = "Insufficient Evidence")]
        INSUFFICIENT_EVIDENCE = 2,

        [Display(Name = "Recording Gap")]
        RECORDING_GAP = 3,

        [Display(Name = "Short Island")]
        SHORT_ISLAND = 4,

        [Display(Name = "Uncorroborated Climb In Descent")]
        UNCORROBORATED_CLIMB_IN_DESCENT = 5
    }
}
