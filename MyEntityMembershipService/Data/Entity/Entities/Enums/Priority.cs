using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data.Entity.Entities.Enums
{
    public enum Priority
    {

        Normal,

        [Display(Name = "High Priority")]
        HighPriority,

        Urgent

    }
}
