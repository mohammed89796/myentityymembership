
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.NewMembershipDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.MembershipRequestServ
{
    public interface IMembershipRequestService
    {
        Task<Response<AddMembershipRequestDto>> AddMembershipRequest(AddMembershipRequestDto request);
        Task<Response<ReturnMembershipRequestDto>> CreateMembershipRequest(CreateMembershipRequestDto RequestDto);
        Task<Response<SubMemberDto>> AddSubMember(SubMemberDto SubMemberDto);
        Task<Response<AddMembershipWaiverRequestDto>> CreateMembershipWaiverRequest(AddMembershipWaiverRequestDto membershipWaiverRequestDto);
        Task<Response<AddMembershipSeparationRequestDto>> CreateMembershipSeparationRequest(AddMembershipSeparationRequestDto separationRequestDto);
        Task<Response<AddMembershipModificationRequestDto>> CreateMembershipModificationRequest(AddMembershipModificationRequestDto modificationRequestDto);
        Task<Response<List<CurrentMembershipDto>>> GetCurrentUserMemberships(string userId, string name);
        Task<Response<CurrentMembershipDto>> GetAuthourizationDataByPackage(int packageId, CurrentMembershipDto currentMembershipDto);
        Task<Response<List<CurrentMembershipRequestDto>>> GetCurrentUserMembershipRequest(string userId, string name);
        Task<Response<GetMembershipRequestDto>> GetMembershipRequest(int Id);
        Task<Response<GetMembershipDto>> GetMembership(int Id);
        Task<Response<List<ActivityDto>>> GetMembershipRequestActivity(int Id);
        Task<Response<GetMembershipRequestDto>> SetPendingStatus(int Id);
        Task<Response<PaginatedFedEntityRequestsList>> GetFedEntityRequests(int tenantID, int typeId, FedEntityRequestFilterDto model);
        Task<Response<PaginatedFedRequestsList>> GetFedRequests(int tenantID, int typeId, FedRequestFilterDto model);
        Task<Response<PaginatedFedEntityRequestsList>> GetFedEntityRequestsByBatchID(int batchID, int typeId, FedEntityRequestByBatchFilterDto model);
        Task<Response<PaginatedFederationEntityBatchList>> GetFedEntityBatchList(int tenantID, int typeId, FedEntityBatchFilterDto filter);
        Task<Response<PaginatedFederationEntityBatchList>> GetFedEntityDraftBatchList(int tenantID, int typeId, FedEntityBatchFilterDto filter);
        Task<Response<PaginatedFederationBatchList>> GetFedBatchList(int tenantID, int typeId, FedBatchFilterDto filter);
        Task<Response<MembershipRequestDetailsDto>> GetRequestDetails(int requestId);
        Task<Response<List<MembershipRequestDetailsByBatchDto>>> GetRequestDetailsByBatchID(int batchId);
        Task<Response<List<MembershipRequestDetailsDto>>> GetBatchDetailedView(int batchId);
        Task<Response<GetFedEntityAnalytics>> GetFedEntityMembershipAnalytics(int tenantID, int typeId);
        Task<Response<GetFedEntityAnalytics>> GetFedMembershipAnalytics(int tenantID, int typeId);
        Task<Response<GetBatchAnalytics>> GetBatchAnalytics(int batchId, int typeId);
        Task<Response<List<GetAllUserMembershipRequests>>> GetCurrentUserMembershipRequestsAsync();
        Task<Response<List<MembershipRequestWithFormDataDto>>> GetCurrentUserMembershipRequestsWithChildrenAsync(int requestId, int? childRequestId);
        Task<Response<List<MembershipRequestWithFormDataDto>>> GetCurrentUserMembershipRequestsWithChildrenByBatchAsync(int batchId);
    }
}
