using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data.Membership.Entities.Enums
{
    public enum PaymentStatus
    {

        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "Pending Payment")]
        PendingPayment,

        [Display(Name = "Pending Confirmation")]
        PendingConfirmation,

        [Display(Name = "Confirmed")]
        Confirmed,

        [Display(Name = "Rejected")]
        Rejected,
    }
}
