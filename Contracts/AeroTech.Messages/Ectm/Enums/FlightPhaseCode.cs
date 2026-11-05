using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum FlightPhaseCode
    {
        [Display(Name = "Ground")]
        GROUND = 1,

        [Display(Name = "Taxi Out")]
        TAXI_OUT = 2,

        [Display(Name = "Takeoff Roll")]
        TAKEOFF_ROLL = 3,

        [Display(Name = "Takeoff")]
        TAKEOFF = 4,

        [Display(Name = "Climb")]
        CLIMB = 5,

        [Display(Name = "Cruise")]
        CRUISE = 6,

        [Display(Name = "Level Flight")]
        LEVEL = 7,

        [Display(Name = "Descent")]
        DESCENT = 8,

        [Display(Name = "Landing Roll")]
        LANDING_ROLL = 9,

        [Display(Name = "Taxi In")]
        TAXI_IN = 10,

        [Display(Name = "Unknown")]
        UNKNOWN = 11
    }
}
