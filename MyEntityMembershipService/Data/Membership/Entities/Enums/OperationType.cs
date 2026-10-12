using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data.Membership.Entities.Enums
{
    public enum OperationType
    {

        [Display(Name = "New Membership")]
        NewMembership = 6,

        [Display(Name = "Transfer")]
        Transfer = 19,

        [Display(Name = "Loan")]
        Loan = 20


    }
}
