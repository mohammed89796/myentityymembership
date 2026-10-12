using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.DTOs.Membership.DashboardDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.PaymentRequest
{
    public class PaymentRequestRepo : GenericRepository<Data.Membership.Entities.PaymentRequest>, IPaymentRequestRepo
    {
        private readonly EntityMembershipDbContext context;

        public PaymentRequestRepo(EntityMembershipDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Data.Membership.Entities.PaymentRequest>> GetPaymmentRequestsBMembershipRequestIdy(int membershipRequestId)
         => await context.PaymentRequests.Where(Payment => Payment.MembershipRequestId == membershipRequestId)
            .ToListAsync();

        public IEnumerable<PaymentSummaryDto> GetTotalAmountPaidGroupedByMonth(int year, int tenantId)
        {
            var paymentsByMonth = context.PaymentRequests
                .Where(p => p.CreationDate.HasValue && p.CreationDate.Value.Year == year && p.TenantId == tenantId) // Check if CreationDate has a value and filter by the given year
                .GroupBy(p => p.CreationDate.HasValue ? p.CreationDate.Value.Month : 0)      // Group by month, or use a default value (e.g., 0) if PaymentDate is null
                .Select(g => new PaymentSummaryDto
                {
                    Month = g.Key,
                    TotalAmountPaid = g.Sum(p => p.TotalAmount) / 1000
                })
                .ToList();

            return paymentsByMonth;
        }


    }
}
