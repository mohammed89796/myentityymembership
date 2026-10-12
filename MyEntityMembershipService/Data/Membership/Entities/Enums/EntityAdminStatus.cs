using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data.Membership.Entities.Enums
{
    public enum EntityAdminStatus
    {
        [Display(Name = "Pending Activation")]
        PendingActivation,

        Active,

        Deactivated,
    }
}
