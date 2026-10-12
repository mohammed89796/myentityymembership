using MyEntityMembershipService.DTOs.Membership.DashboardDtos;
using MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.RequestPaymentServ
{
    public interface IPaymentRequestService
    {
       
        Task<Response<PaymentRequestDto>> SubmitPaymentComments(int tenantId, int paymentId, string comments);
        Task<Response<DashboardSummaryDto>> GetDashboardSummaryAsync(int year, int tenantId);


    }
}
