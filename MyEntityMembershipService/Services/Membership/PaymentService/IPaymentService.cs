using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.DTOs.Membership.FederationDtos;
using MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos;
using MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos.FedrationEntitiesPaymentRequests;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<Response<object>> CreatePaymentRequestAsync(AddPaymentRequestDto dto);
        Task<Response<object>> CreatePindingPaymentRequest(PindingRequestsDto dto);

        Task<Response<PaginationToReturnDto<GetAllFedrationPaymentRequestToReturnDto>>> GetAllFedrationPaymentRequestsAsync(GetFedrationPaymentRequestDto dto);
        Task<Response<PaginationToReturnDto<GetAllFedrationEntitiesPaymentRequestToReturnDto>>> GetAllFedrationEntitiesPaymentRequestsAsync(GetFedrationEntityPaymentRequestDto dto);

        Task<Response<GetFedrationPaymentRequestToReturnDto>> GetFedrationPaymentRequestByIdAsync(int paymentRequestId);

        Task<Response<int>> GetBatchFeesByIdAsync(int batchId);

        Task<Response<string>> ConfirmPaymentRequestAsync(ConfirmPaymentDto dto);

        Task<Response<object>> GetAllEntityPaymentRequestsAsync(GetPaginationDto dto);
        Task<Response<PaginationToReturnDto<GetAllEntityPaymentRequestToReturnDto>>> GetAllEntityPaymentRequest(GetEntitiyPaymentRequestDto dto);
        Task<Response<EntityPaymentRequestStatusSummaryDto>> GetPaymentRequestStatusSummaryAsync(int entityId);
        Task<Response<FederationPaymentSummaryDto>> GetFederationPaymentSummaryAsync();

        Task<Response<string>> GenereatePaymentRequests(List<int> requestIds);

        Task<Response<string>> EntityPayNowAsync(EntityPayNowDto dto);

        Task<Response<List<object>>> GetEntityBankAcounts();

        Task<Response<EntityPaymentSummaryDto>> EntityPaymentSummary(List<int> memberRequestIds);
        Task<Response<EntityPaymentSummaryDto>> GetFedrationPaymentSummariesByRequestIdsAsync(List<int> paymentRequestIds);

        Task<Response<string>> RejectPaymentAsync(RejectPaymentDto dto);
    }
}
