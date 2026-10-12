using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.PaymentInvoiceServ
{
    public interface IPaymentInvoiceService
    {
        Task<Response<PaymentInvoiceDetailsDto>> GetPaymentInvoiceDetails(int? id, int? transactionId);

        Task<Response<List<PaymentInvoiceDto>>> GetAllUserInvoice(string userId);
     


        Task<Response<CreatePaymentIntentionResult>> CreateorUpdatePaymentIntentId(PaymentRequestDto paymentRequestDto);
        Task<Response<List<PaymentInvoiceDto>>> GetAllInvoices();

        Task<Response<string>> CheckOut(PaymentWebhookDto webhookDto);

        
    }
}
