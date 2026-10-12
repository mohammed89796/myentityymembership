using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data.Membership.Entities.Enums
{
    public enum PaymentType
    {
        [Display(Name = "Cash")]
        Cash,

        [Display(Name = "Instapay")]
        Instapay,

        [Display(Name = "Bank Transfer")]
        BankTransfer,

        [Display(Name = "E-wallet")]
        Ewallet,

        [Display(Name = "Online")]
        Online,

    }
}
