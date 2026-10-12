using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.Repositories.GenericRepo;
using System;

namespace MyEntityMembershipService.Repositories.Membership.MembershipRequestRepository
{
    public interface IMembershipRequestRepository : IGenericRepository<MembershipRequest>
    {
        Task<List<MembershipRequest>> GetAllRequestsByTenantId(int tenantId);
        Task<GetAllRequestsGroupedResponseDto> GetAllRequestsGrouped(int tenantId);
        Task<List<MembershipRequestDto>> GetRequestsByTypeID(int reqesrtType, int tenantId);
        Task<List<MembershipRequest>> GetUserMembershipRequests(string userId);
        Task<List<MembershipRequest>> GetMembershipRequestsByName(string userId, string name);
        Task<MembershipRequest> GetMembershipRequestsById(int tenantId, int requestId);

        Task<List<MembershipRequestComment>> GetMembershipRequestComments(int Id);
        Task<List<Data.Membership.Entities.PaymentRequest>> GetMembershipRequestPayments(int Id);

        Task<List<MembershipRequest>> GetMembershipSubmembers(int Id);
        Task<List<MembershipRequest>> GeMembershipRequestByPackageId(int Id);

        Task<List<MembershipRequest>> GetRequestsOfMembershipByType(int requestTypeId, int membershipId);
        Task<List<MembershipRequest>> GetResolvedRequestsOfMembership(int membershipId);
        Task<List<MembershipRequestBackgroundJobs>> GetMembershipRequestJobs(int requestId);
        Task<MembershipRequest> GetMembershipRequestWithStatusHistory(int requestId);
        Task<PaginatedFedEntityRequestsList> GetFedEntityRequestsByTenantId(int tenantId, int typeId, FedEntityRequestFilterDto filter);
        Task<PaginatedFedRequestsList> GetFedRequestsByTenantId(int tenantId,List<int>areaIds, int typeId, FedRequestFilterDto filter);
        Task<PaginatedFedEntityRequestsList> GetFedEntityRequestsByBatchId(int batchID, int typeId, FedEntityRequestByBatchFilterDto filter);
        Task<PaginatedFederationEntityBatchList> GetFedEntityBatchesByTenantId(int tenantID, int typeId,FedEntityBatchFilterDto filter);
        Task<PaginatedFederationEntityBatchList> GetFedEntityDraftBatchesByTenantId(int tenantID, int typeId, FedEntityBatchFilterDto filter);
        Task<PaginatedFederationBatchList> GetFedBatchesByTenantId(int tenantID, List<int> areaIds, int typeId, FedBatchFilterDto filter);
        Task<MembershipRequestDetailsDto?> GetMembershipRequestDetails(int requestId);
        Task<List<MembershipRequestDetailsByBatchDto>> GetMembershipRequestsByBatch(int batchId);
        Task<List<MembershipRequestDetailsDto>> GetBatchDetailedViewAsync(int batchId);
        Task<GetFedEntityAnalytics> GetFedEntityMembershipAnalyticsAsync(int tenantID,int typeId);
        Task<GetFedEntityAnalytics> GetFedMembershipAnalyticsAsync(int tenantID ,List<int> areaIds,int typeId);
        Task<GetBatchAnalytics> GetBatchAnalyticsAsync(int batchId ,int typeId);
    }
}
