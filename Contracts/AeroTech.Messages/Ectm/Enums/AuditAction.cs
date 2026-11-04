using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum AuditAction
    {
        [Display(Name = "Created")]
        CREATED = 1,

        [Display(Name = "Updated")]
        UPDATED = 2,

        [Display(Name = "Deleted")]
        DELETED = 3
    }
}
