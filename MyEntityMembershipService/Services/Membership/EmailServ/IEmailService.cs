using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Services.Membership.EmailServ
{
    public interface IEmailService
    {
        void SendInvoiceConfirmationEmail(PaymentInvoice paymentInvoice);
    }
}
