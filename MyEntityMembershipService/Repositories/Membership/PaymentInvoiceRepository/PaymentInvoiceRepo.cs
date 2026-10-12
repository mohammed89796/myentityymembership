using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.PaymentInvoiceRepository
{
    public class PaymentInvoiceRepo : GenericRepository<PaymentInvoice>, IPaymentInvoiceRepo
    {
        private readonly EntityMembershipDbContext _context;

        public PaymentInvoiceRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaymentInvoice> GetPaymentInvoiceDetailsAsync(int id)
            => await _context.PaymentInvoices.Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<PaymentInvoice> GetPaymentInvoiceDetailsWithOrderIdAsync(int transactionId)
            => await _context.PaymentInvoices.Include(p => p.Items).FirstOrDefaultAsync(p => p.TransactionId == transactionId);

        public async Task<PaymentInvoice> GetPaymentInvoiceDetailsWithIntentionIdAsync(int IntentionId)
            => await _context.PaymentInvoices.Include(p => p.Items).FirstOrDefaultAsync(p => p.PaymentIntentId == IntentionId);

        public Task<List<PaymentInvoice>> GetUserPayments(string userId)
            => _context.PaymentInvoices.AsNoTracking().Where(p => p.UserId == userId).ToListAsync();


        //public async Task<PaymentInvoice> GetPaymentInvoiceDetailsAsync(int id, int typeId)
        //    => await _context.PaymentInvoices.Include(p => p.Items
        //                                     .Where(i => i.PaymentTypeId == typeId))
        //                                     .FirstOrDefaultAsync(p => p.Id == id);


    }
}
