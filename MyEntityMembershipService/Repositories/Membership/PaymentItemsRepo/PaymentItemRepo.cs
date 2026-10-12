using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.PaymentItemsRepo
{
    public class PaymentItemRepo : GenericRepository<PaymentItem>, IPaymentItemRepo
    {
        private readonly EntityMembershipDbContext _context;

        public PaymentItemRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<PaymentItem>> GetAllPaymentItemsbyPaymentType(PaymentInvoiceType invoiceType)
            => await _context.PaymentItems.Include(x => x.PaymentInvoice)
                                          .Where(i => i.PaymentType == invoiceType).ToListAsync();

        public async Task<List<PaymentItem>> GetAllPaymentItemsbyPaymentTypeAndTypeId(PaymentInvoiceType invoiceType, int typeId)
            => await _context.PaymentItems.Include(x => x.PaymentInvoice)
                                          .Where(i => i.PaymentType == invoiceType && i.PaymentTypeId == typeId).ToListAsync();
    }
}
