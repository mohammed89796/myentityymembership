using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.PaymentItemsRepo
{
    public interface IPaymentItemRepo : IGenericRepository<PaymentItem>
    {
        Task<List<PaymentItem>> GetAllPaymentItemsbyPaymentTypeAndTypeId(PaymentInvoiceType invoiceType, int typeId);
        Task<List<PaymentItem>> GetAllPaymentItemsbyPaymentType(PaymentInvoiceType invoiceType);


    }
}
