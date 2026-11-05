using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum CalibrationStatus
    {
        [Display(Name = "Candidate (not calibrated)")]
        CANDIDATE = 1,

        [Display(Name = "Calibrated")]
        CALIBRATED = 2
    }
}
