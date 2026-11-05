using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum EngineeringSignOff
    {
        [Display(Name = "Engineering Sign-off Pending")]
        PENDING = 1,

        [Display(Name = "Approved by the Flight Engineer")]
        APPROVED = 2
    }
}
