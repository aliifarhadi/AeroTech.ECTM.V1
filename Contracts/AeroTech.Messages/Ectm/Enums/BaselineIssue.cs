using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum BaselineIssue
    {
        [Display(Name = "No Parameter")]
        NO_PARAMETER = 1,

        [Display(Name = "Value Missing")]
        VALUE_MISSING = 2
    }
}
