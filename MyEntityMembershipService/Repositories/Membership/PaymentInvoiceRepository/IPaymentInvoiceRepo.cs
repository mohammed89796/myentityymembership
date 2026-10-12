using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.PaymentInvoiceRepository
{
    public interface IPaymentInvoiceRepo : IGenericRepository<PaymentInvoice>
    {
        Task<PaymentInvoice> GetPaymentInvoiceDetailsAsync(int id);
        Task<PaymentInvoice> GetPaymentInvoiceDetailsWithOrderIdAsync(int transactionId);
        Task<PaymentInvoice> GetPaymentInvoiceDetailsWithIntentionIdAsync(int IntentionId);
    


        Task<List<PaymentInvoice>> GetUserPayments(string userId);
    }
}
