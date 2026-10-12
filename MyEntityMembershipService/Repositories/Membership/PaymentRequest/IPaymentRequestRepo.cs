using MyEntityMembershipService.DTOs.Membership.DashboardDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.PaymentRequest
{
    public interface IPaymentRequestRepo: IGenericRepository<Data.Membership.Entities.PaymentRequest>
    {
        Task<List<Data.Membership.Entities.PaymentRequest>> GetPaymmentRequestsBMembershipRequestIdy(int MembershipRequestId);
        IEnumerable<PaymentSummaryDto> GetTotalAmountPaidGroupedByMonth(int year, int tenantId);

    }
}
