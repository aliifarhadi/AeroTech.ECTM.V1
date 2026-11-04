using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum MaintenanceEventType
    {
        [Display(Name = "Wash")]
        WASH = 1,

        [Display(Name = "Borescope")]
        BORESCOPE = 2,

        [Display(Name = "Hot Section Inspection")]
        HSI = 3,

        [Display(Name = "Shop Visit")]
        SHOP_VISIT = 4,

        [Display(Name = "Install")]
        INSTALL = 5,

        [Display(Name = "Remove")]
        REMOVE = 6,

        [Display(Name = "Other")]
        OTHER = 7
    }
}
