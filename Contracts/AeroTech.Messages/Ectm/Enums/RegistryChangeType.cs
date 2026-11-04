using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum RegistryChangeType
    {
        [Display(Name = "Installed")]
        INSTALLED = 1,

        [Display(Name = "Removed")]
        REMOVED = 2
    }
}
