using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos;
using MyEntityMembershipService.DTOs.Membership.SwimmingMembershipRequestDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.MembershipRequestWebServ
{
	public interface IMembershipRequestWebService
	{
		Task<Response<List<MembershipRequest>>> GetAllRequest();

		Task<Response<MembershipRequestDto>> GetMembershipRequestById(int tenantId, int requestId);

		Task<Response<GetAllRequestsGroupedResponseDto>> GetAllRequestsGrouped(int tenantId);

		Task<Response<PaginationToReturnDto<MembershipRequestDto>>> GetRequestsByTenantId(NewMembershipRequestsDto? newMembershipRequestsDto, int tenantId, int pageSize, int pageNumber);

		Task<Response<PaginationToReturnDto<MembershipRequestDto>>> GetAllPendingPaymentRequests(PendingPaymentRequestDto? pendingPaymentRequestDto, int tenantId, int pageSize, int pageNumber);

		Task<Response<MembershipRequestDto>> UpdateRequestStatus(int tenantId, int requestId, RequestStatus newStatus);
		Task<Response<MembershipRequestDto>> UpdateRequestStatusWithComment(UpdateRequestStatusWithComment model);

		Task<Response<MembershipRequestDto>> UpdateParentWithSubmembersRequestStatus(int tenantId, int requestId);
        Task<Response<MembershipRequestDto>> SubmitCommentsAndJustfications(SubmitCommentsAndJustficationsDto model);
		Task<Response<PaginationToReturnDto<MembershipRequestDto>>> SortMembershipRequests(string sortBy, string order, int pageSize, int pageNumber);
		Task<Response<PaginationToReturnDto<MembershipRequestDto>>> GetFilteredTenantRequests(int tenantId, int requestStatus, int pageSize, int pageNumber);
		Task<Response<PaginationToReturnDto<MembershipRequestDto>>> SearchMembershipRequests(string searchTerm, int pageSize, int pageNumber);

        Task<Response<MembershipRequestDto>> UpdateRequestAndMembershipStatus(int tenantId, int requestId, RequestStatus newStatus);
		Task<Response<List<MembershipRequestDto>>> GeMembershipRequestssByPackageId(int PackageId);
		Task<Response<object>> GetRevenuesPerMembershipsPackage(int tenantId, int month, int year);
		Task<Response<MembershipRequestDto>> ApproveRequestStatus(int requestId);
		Task<Response<MembershipRequestDto>> ApproveRequestAndCreateMembership(int requestId);
		Task<Response<MembershipRequestDto>> ApproveRequestAndTransferOrLonedMembership(int requestId);
        Task<Response<MembershipRequestDto>> RejectRequestStatus(int requestId, RejectionReason rejectionReason);

        Task<Response<PagedResult<ConfirmedMembershipRequestDto>>> GetConfirmedMembershipsAsync(int entityId, int pageIndex, int pageSize);
        Task<Response<EntityMembershipAnalyticsDto>> GetEntityMembershipAnalytics(int tenantId);

		Task<Response<string>> SoftDeleteMembershipRequestAsync(int requestId);
		Task<Response<GetEntityMembershipRequestDto>> GetMembershipRequestById(int requestId);
		Task<Response<PaginationToReturnDto<GetAllBatches>>> GetAllBatchesAsync(GetAllBatchesParamDto input);
		Task<Response<PaginationToReturnDto<ChildRequestDto>>> GetAllChildRequestsAsync(GetAllChildRequestParametersDto input);
		Task<Response<PaginationToReturnDto<ChildRequestDto>>> GetAllChildRequestsByBatchAsync(GetAllChildRequestParametersDto input, int batchId);
		Task<Response<CurrentMembershipDto>> ApproveRequestAndCreateMembershipWithDates(ApproveMembershipRequestDto approveMembershipRequestDto);
        Task<Response<string>> SetBatchRequestsToPending(int batchId);

    }
}