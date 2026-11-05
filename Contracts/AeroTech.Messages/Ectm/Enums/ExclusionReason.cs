using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ExclusionReason
    {
        [Display(Name = "Data Quality Makes the Flight Unusable")]
        DATA_QUALITY_UNUSABLE = 1,

        [Display(Name = "Aircraft or Engine Identity Cannot Be Confirmed")]
        IDENTITY_UNCONFIRMED = 2,

        [Display(Name = "Duplicate of Another Recording")]
        DUPLICATE_RECORDING = 3,

        [Display(Name = "Recording Misses the Takeoff or the Cruise")]
        INCOMPLETE_RECORDING = 4,

        [Display(Name = "Engine Sensor or Recorder Fault")]
        SENSOR_OR_RECORDER_FAULT = 5,

        [Display(Name = "Test - Ferry - Training or Maintenance Flight")]
        TEST_FERRY_OR_TRAINING_FLIGHT = 6,

        [Display(Name = "Abnormal Operation (Engine Out - Rejected Takeoff - Go-around)")]
        ABNORMAL_OPERATION = 7,

        [Display(Name = "Engine Event Under Investigation")]
        ENGINE_EVENT_UNDER_INVESTIGATION = 8,

        [Display(Name = "Other - Explained in the Note")]
        OTHER = 9
    }
}
