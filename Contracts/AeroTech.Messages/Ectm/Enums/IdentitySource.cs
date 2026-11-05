using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum IdentitySource
    {
        [Display(Name = "File Name")]
        FILE_NAME = 1,

        [Display(Name = "Columns")]
        COLUMNS = 2,

        [Display(Name = "User Default")]
        USER_DEFAULT = 3,

        [Display(Name = "User")]
        USER = 4
    }
}
