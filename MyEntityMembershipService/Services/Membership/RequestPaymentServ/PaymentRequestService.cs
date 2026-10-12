using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.DashboardDtos;
using MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.PaymentRequest;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Membership.RequestPaymentServ
{
    public class PaymentRequestService : IPaymentRequestService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        private readonly IPaymentRequestRepo paymentRequestRepo;

        public PaymentRequestService(IUnitOfWork unitOfWork, IMapper mapper,
                                        IConfiguration configuration
, IPaymentRequestRepo paymentRequestRepo)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.configuration = configuration;
            this.paymentRequestRepo = paymentRequestRepo;
        }

        public async Task<Response<PaymentRequestDto>> SubmitPaymentComments(int tenantId, int paymentId, string comments)
        {
            try
            {
                // Retrieve the payment by its ID
                var payment = await unitOfWork.Repository<PaymentRequest>().GetByIdAsync(paymentId);

                // Check if the tenant ID matches
                if (payment.TenantId != tenantId)
                {
                    return new ResponseHandler().BadRequest<PaymentRequestDto>("Tenant ID does not match.");
                }

                // Update the payment status
                payment.Comments = comments;

                // Update the specific field
                unitOfWork.Repository<PaymentRequest>().UpdateField(payment, r => r.Comments);
                await unitOfWork.Save();

                // Map the updated entity to a DTO
                var updatedRequestDto = mapper.Map<PaymentRequestDto>(payment);
                return new ResponseHandler().Success(updatedRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaymentRequestDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<DashboardSummaryDto>> GetDashboardSummaryAsync(int year, int tenantId)
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12)
        .Select(month => new PaymentSummaryDto(month))
        .ToList();

                // Get total program payments grouped by month
                var totalProgramPayments = paymentRequestRepo.GetTotalAmountPaidGroupedByMonth(year, tenantId);

                // Update the TotalAmountPaid for each month based on retrieved payments
                foreach (var payment in totalProgramPayments)
                {
                    var existingPayment = allMonths.FirstOrDefault(m => m.Month == payment.Month);
                    if (existingPayment != null)
                    {
                        existingPayment.TotalAmountPaid = payment.TotalAmountPaid; // Assign total amount for the month
                    }
                }

                // Create the dashboard summary
                var dashboardSummary = new DashboardSummaryDto
                {
                    TotalMembershipPayment = allMonths, // Assign all months with updated totals
                    MinValue = 0,
                    MaxValue = 250,
                    Interval = 50
                };

                // Return the response
                return new ResponseHandler().Success(dashboardSummary);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return a bad request response
                return new ResponseHandler().BadRequest<DashboardSummaryDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }


    }
}

