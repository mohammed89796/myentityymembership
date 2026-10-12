using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos;
using MyEntityMembershipService.DTOs.Membership.SwimmingMembershipRequestDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.MembershipRequestServ;
using MyEntityMembershipService.Services.Membership.MembershipRequestWebServ;

using System.Security.Claims;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
	[Route("api/{version:apiVersion}/[controller]")]
	[Route("api/v{version:apiVersion}/[controller]")]

	[ApiVersion("1.0")]
	[ApiController]
    [Authorize]

    public class MembershipRequestController : ControllerBase
	{
		private readonly IMembershipRequestService membershipRequestService;
		private readonly IMembershipRequestWebService membershipRequestWebService;


		public MembershipRequestController(IMembershipRequestService membershipRequestService, IMembershipRequestWebService membershipRequestWebService)
		{
			this.membershipRequestService = membershipRequestService;
			this.membershipRequestWebService = membershipRequestWebService;
		}

		[HttpGet("GetAllTenantRequestsGrouped")]
		public async Task<IResult> GetAllTenantRequestsGrouped(int tenantId)
		{
			var result = await membershipRequestWebService.GetAllRequestsGrouped(tenantId);
			return result.GetResponse();
		}

		[HttpPost("CreateMembershipRequest")]
		public async Task<IResult> CreateMembershipRequest([FromForm] CreateMembershipRequestDto membershipRequestDto)
		{
			var result = await membershipRequestService.CreateMembershipRequest(membershipRequestDto);
			return result.GetResponse();
		}
		[HttpPost("AddSubMember")]
		public async Task<IResult> AddSubMember([FromForm] SubMemberDto subMemberDto)
		{
			var result = await membershipRequestService.AddSubMember(subMemberDto);
			return result.GetResponse();
		}
		[HttpPost("AddMembershipRequest")]
		public async Task<IResult> AddMembershipRequest(AddMembershipRequestDto membershipRequestDto)
		{
			var result = await membershipRequestService.AddMembershipRequest(membershipRequestDto);
			return result.GetResponse();
		}
		[HttpPost("AddMembershipWaiverRequest")]
		public async Task<IResult> AddMembershipWaiverRequest(AddMembershipWaiverRequestDto membershipWaiverRequestDto)
		{
			var result = await membershipRequestService.CreateMembershipWaiverRequest(membershipWaiverRequestDto);
			return result.GetResponse();
		}
		[HttpPost("AddMembershipSeparationRequest")]
		public async Task<IResult> AddMembershipSeparationRequest(AddMembershipSeparationRequestDto separationRequestDto)
		{
			var result = await membershipRequestService.CreateMembershipSeparationRequest(separationRequestDto);
			return result.GetResponse();
		}
		[HttpPost("AddMembershipModificationRequest")]
		public async Task<IResult> AddMembershipModificationRequest(AddMembershipModificationRequestDto modificationRequestDto)
		{
			var result = await membershipRequestService.CreateMembershipModificationRequest(modificationRequestDto);
			return result.GetResponse();
		}

		[HttpPost("GetAllMembershipRequestsByTenantId")]
		public async Task<IResult> GetAllMembershipRequestsByTenantId([FromBody] NewMembershipRequestsDto? newMembershipRequestsDto, [FromQuery] int tenantId, int pageNumber, int pageSize)
		{
			var result = await membershipRequestWebService.GetRequestsByTenantId(newMembershipRequestsDto, tenantId, pageSize, pageNumber);
			return result.GetResponse();
		}

		[HttpPost("GetAllPendingPaymentRequests")]
		public async Task<IResult> GetAllPendingPaymentRequests([FromBody] PendingPaymentRequestDto? pendingPaymentRequestDto, [FromQuery] int tenantId, int pageNumber, int pageSize)
		{
			var result = await membershipRequestWebService.GetAllPendingPaymentRequests(pendingPaymentRequestDto, tenantId, pageSize, pageNumber);
			return result.GetResponse();
		}
		[HttpGet("GetCurrentUserMemberships")]
		public async Task<IResult> GetCurrentUserMemberships(string userId, string? name)
		{
			var result = await membershipRequestService.GetCurrentUserMemberships(userId, name);
			return result.GetResponse();
		}
		[HttpGet("GetAuthourizationDataByPackage")]
		public async Task<IResult> GetAuthourizationDataByPackage(int packageId)
		{
			var currentMembershipDto = new CurrentMembershipDto();
			var result = await membershipRequestService.GetAuthourizationDataByPackage(packageId, currentMembershipDto);
			return result.GetResponse();
		}
		[HttpGet("GetCurrentUserMembershipRequests")]
		public async Task<IResult> GetCurrentUserMembershipRequests(string userId, string? name)
		{
			var result = await membershipRequestService.GetCurrentUserMembershipRequest(userId, name);
			return result.GetResponse();
		}
		[HttpGet("GetMembershipRequestById")]
		public async Task<IResult> GetCurrentUserMembershipRequests(int tenantId, int requestId)
		{
			var result = await membershipRequestWebService.GetMembershipRequestById(tenantId, requestId);
			return result.GetResponse();
		}

		[HttpPut("Aprove-update-status-PendingPayment")]
		public async Task<IResult> UpdateStatusPendingPayment(int tenantId, int requestId)
		{
			var result = await membershipRequestWebService.UpdateRequestStatus(tenantId, requestId, RequestStatus.PendingPayment);
			return result.GetResponse();
		}
		[HttpPut("Aprove-update-status-RequestResolved")]
		public async Task<IResult> UpdateStatusRequestResolved(int tenantId, int requestId)
		{
			var result = await membershipRequestWebService.UpdateRequestAndMembershipStatus(tenantId, requestId, RequestStatus.RequestResolved);
			return result.GetResponse();
		}
		[HttpPut("update-status-Reject-Request")]
		public async Task<IResult> UpdateStatusPaymentComplete(int tenantId, int requestId)
		{
			var result = await membershipRequestWebService.UpdateRequestStatus(tenantId, requestId, RequestStatus.Rejected);
			return result.GetResponse();
		}

        [HttpPut("rejectrequest")]
        public async Task<IResult> RejectRequestStatus(int requestId, RejectionReason rejectionReason)
        {
            var result = await membershipRequestWebService.RejectRequestStatus(requestId,rejectionReason);
            return result.GetResponse();
        }

        [HttpPut("Aprove-update-status-PaymentComplete")]
		public async Task<IResult> UpdateStatusAprove(UpdateRequestStatusWithComment model)
		{
			model.newStatus = RequestStatus.PaymentComplete;

			var result = await membershipRequestWebService.UpdateRequestStatusWithComment(model);
			return result.GetResponse();
		}

		[HttpPut("update-status-Reject")]
		public async Task<IResult> UpdateStatusReject(UpdateRequestStatusWithComment model)
		{
			model.newStatus = RequestStatus.Rejected;
			var result = await membershipRequestWebService.UpdateRequestStatusWithComment(model);
			return result.GetResponse();
		}

		[HttpPut("submit-comments-And-justfications")]
		public async Task<IResult> SubmitCommentsAndJustfications(SubmitCommentsAndJustficationsDto model)
		{
			var result = await membershipRequestWebService.SubmitCommentsAndJustfications(model);
			return result.GetResponse();
		}

		[HttpGet("sort-membership-requests")]
		public async Task<IResult> SortMembershipRequests([FromQuery] string sortBy, string order, int pageNumber, int pageSize)
		{
			var result = await membershipRequestWebService.SortMembershipRequests(sortBy, order, pageSize, pageNumber);
			return result.GetResponse();
		}

		[HttpGet("FilterByRequestSatus")]
		public async Task<IResult> GetFilteredRequestsBySatus([FromQuery] int tenantId, int requestStatus, int pageNumber, int pageSize)
		{
			var result = await membershipRequestWebService.GetFilteredTenantRequests(tenantId, requestStatus, pageSize, pageNumber);
			return result.GetResponse();
		}

		[HttpGet("search-membership-requests")]
		public async Task<IResult> SearchMembershipRequests([FromQuery] string searchTerm, int pageNumber, int pageSize)
		{
			var result = await membershipRequestWebService.SearchMembershipRequests(searchTerm, pageSize, pageNumber);
			return result.GetResponse();
		}

		[HttpGet("GetMembershipRequest")]
		public async Task<IResult> GetMembershipRequest(int Id)
		{
			var result = await membershipRequestService.GetMembershipRequest(Id);
			return result.GetResponse();
		}

		[HttpGet("GetMembership")]
		public async Task<IResult> GetMembership(int Id)
		{
			var result = await membershipRequestService.GetMembership(Id);
			return result.GetResponse();
		}
		[HttpGet("GetMembershipRequestActivity")]
		public async Task<IResult> GetMembershipRequestActivity(int Id)
		{
			var result = await membershipRequestService.GetMembershipRequestActivity(Id);
			return result.GetResponse();
		}

		[HttpPut("SetSubmittedStatus")]
		public async Task<IResult> SetSubmittedStatus(int Id)
		{
			var result = await membershipRequestService.SetPendingStatus(Id);
			return result.GetResponse();
		}
		[HttpPut("UpdateParentWithSubmembersRequestStatus")]
		public async Task<IResult> UpdateParentWithSubmembersRequestStatus(int tenantId, int requestId)
		{
			var result = await membershipRequestWebService.UpdateParentWithSubmembersRequestStatus(tenantId, requestId);
			return result.GetResponse();
		}

		[HttpGet("GeMembershipRequestssByPackageId")]
		public async Task<IResult> GeMembershipRequestssByPackageId(int packageId)
		{
			var result = await membershipRequestWebService.GeMembershipRequestssByPackageId(packageId);
			return result.GetResponse();
		}

		[HttpGet("GetRevenuesPerMembershipsPackage/{tenantId}/{month}/{year}")]
		public async Task<IResult> GetRevenuesPerMembershipsPackage(int tenantId, int month, int year)
		{
			var result = await membershipRequestWebService.GetRevenuesPerMembershipsPackage(tenantId, month, year);
			return result.GetResponse();
		}

		[HttpPost("GetFedEntityRequests/{tenantID}/{typeId}")]
		public async Task<IResult> GetFedEntityRequests(int tenantID, int typeId, [FromBody] FedEntityRequestFilterDto model)
		{
			var result = await membershipRequestService.GetFedEntityRequests(tenantID,typeId, model);
			return result.GetResponse();
		}
		[HttpPost("GetFedRequests/{tenantID}/{typeId}")]
		public async Task<IResult> GetFedRequests(int tenantID, int typeId, [FromBody] FedRequestFilterDto model)
		{
			var result = await membershipRequestService.GetFedRequests(tenantID,typeId, model);
			return result.GetResponse();
		}
		[HttpPost("GetFedEntityRequestsByBatchID/{batchID}/{typeId}")]
		public async Task<IResult> GetFedEntityRequestsByBatchID(int batchID, int typeId, [FromBody] FedEntityRequestByBatchFilterDto model)
		{
			var result = await membershipRequestService.GetFedEntityRequestsByBatchID(batchID,typeId, model);
			return result.GetResponse();
		}
		[HttpPost("GetFedEntityBatchList/{tenantID}/{typeId}")]
		public async Task<IResult> GetFedEntityBatchList(int tenantID,int typeId, [FromBody] FedEntityBatchFilterDto filter)
		{
			var result = await membershipRequestService.GetFedEntityBatchList(tenantID, typeId, filter);
			return result.GetResponse();
		}
		[HttpPost("GetFedEntityDraftBatchList/{tenantID}/{typeId}")]
		public async Task<IResult> GetFedEntityDraftBatchList(int tenantID, int typeId, [FromBody] FedEntityBatchFilterDto filter)
		{
			var result = await membershipRequestService.GetFedEntityDraftBatchList(tenantID,typeId ,filter);
			return result.GetResponse();
		}
		[HttpPost("GetFedBatchList/{tenantID}/{typeId}")]
		public async Task<IResult> GetFedBatchList(int tenantID, int typeId, [FromBody] FedBatchFilterDto filter)
		{
			var result = await membershipRequestService.GetFedBatchList(tenantID, typeId,filter);
			return result.GetResponse();
		}
		[HttpGet("GetMembershipRequestDetails/{requestId}")]
		public async Task<IResult> GetMembershipRequestDetails(int requestId)
		{
			var result = await membershipRequestService.GetRequestDetails(requestId);
			return result.GetResponse();
		}
		[HttpGet("GetRequestDetailsByBatchID/{batchId}")]
		public async Task<IResult> GetRequestDetailsByBatchID(int batchId)
		{
			var result = await membershipRequestService.GetRequestDetailsByBatchID(batchId);
			return result.GetResponse();
		}
		[HttpGet("GetBatchDetailedView/{batchId}")]
		public async Task<IResult> GetBatchDetailedView(int batchId)
		{
			var result = await membershipRequestService.GetBatchDetailedView(batchId);
			return result.GetResponse();
		}
		[HttpGet("GetFedEntityMembershipAnalytics/{tenantId}/{typeId}")]
		public async Task<IResult> GetFedEntityMembershipAnalytics(int tenantId, int typeId)
		{
			var result = await membershipRequestService.GetFedEntityMembershipAnalytics(tenantId,typeId);
			return result.GetResponse();
		}
		[HttpGet("GetFedMembershipAnalytics/{tenantId}/{typeId}")]
		public async Task<IResult> GetFedMembershipAnalytics(int tenantId,int typeId)
		{
			var result = await membershipRequestService.GetFedMembershipAnalytics(tenantId, typeId);
			return result.GetResponse();
		}
		[HttpGet("GetBatchAnalytics/{batchId}/{typeId}")]
		public async Task<IResult> GetBatchAnalytics(int batchId,int typeId)
		{
			var result = await membershipRequestService.GetBatchAnalytics(batchId, typeId);
			return result.GetResponse();
		}

        [HttpPut("ApproveRequestStatus")]
        public async Task<IResult> ApproveRequestStatus(int requestId)
        {
            var result = await membershipRequestWebService.ApproveRequestAndTransferOrLonedMembership(requestId);
            return result.GetResponse();
        }
        [HttpPut("ApproveRequestAndCreateMembership")]
        public async Task<IResult> ApproveRequestAndCreateMembership(int requestId)
        {
            var result = await membershipRequestWebService.ApproveRequestAndCreateMembership(requestId);
            return result.GetResponse();
        }
        [HttpPut("ApproveRequestAndCreateMembershipWithDates")]
        public async Task<IResult> ApproveRequestAndCreateMembershipWithDates(ApproveMembershipRequestDto approveMembershipRequestDto)
        {
            var result = await membershipRequestWebService.ApproveRequestAndCreateMembershipWithDates(approveMembershipRequestDto);
            return result.GetResponse();
        }
        [HttpGet("GetConfirmedMemberships/{entityId}")]

        public async Task<IResult> GetConfirmedMemberships(int entityId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await membershipRequestWebService.GetConfirmedMembershipsAsync(entityId, pageIndex, pageSize);
            return result.GetResponse();
        }

        [HttpGet("GetEntityMembershipAnalytics/{tenantId}")]
        public async Task<IResult> GetEntityMembershipAnalytics(int tenantId)
        {
            var result = await membershipRequestWebService.GetEntityMembershipAnalytics(tenantId);
            return result.GetResponse();
        }

        [HttpDelete("DeleteRequest/{requestId}")]
        public async Task<IResult> SoftDeleteMembershipRequestAsync(int requestId)
        {
            var result = await membershipRequestWebService.SoftDeleteMembershipRequestAsync(requestId);
            return result.GetResponse();
        }
        [HttpGet("GetMembershipRequestById/{requestId}")]
        public async Task<IResult> GetMembershipRequestById(int requestId)
        {
            var result = await membershipRequestWebService.GetMembershipRequestById(requestId);
            return result.GetResponse();
        }
        [HttpGet("GetSwimmingCurrentUserMembershipRequests")]
        public async Task<IResult> GetCurrentUserMembershipRequests()
        {
            var result = await membershipRequestService.GetCurrentUserMembershipRequestsAsync();
            return result.GetResponse();
        }
        [HttpGet("GetCurrentUserMembershipRequestsWithChildren")]
        public async Task<IResult> GetCurrentUserMembershipRequestsWithChildren(int requestId, int? childRequestId)
        {
            var result = await membershipRequestService.GetCurrentUserMembershipRequestsWithChildrenAsync(requestId,childRequestId);
            return result.GetResponse();
        }
        [HttpGet("GetCurrentUserMembershipRequestsWithChildrenByBatch")]
        public async Task<IResult> GetCurrentUserMembershipRequestsWithChildrenByBatch(int requestId)
        {
            var result = await membershipRequestService.GetCurrentUserMembershipRequestsWithChildrenByBatchAsync(requestId);
            return result.GetResponse();
        }
        [HttpGet("GetAllBatches")]
        public async Task<IResult> GetBatchAnalytics([FromQuery] GetAllBatchesParamDto allBatchesParamDto )
        {
            var result = await membershipRequestWebService.GetAllBatchesAsync(allBatchesParamDto);
            return result.GetResponse();
        }
        [HttpGet("GetAllChildRequests")]
        public async Task<IResult> GetAllChildRequestsAsync([FromQuery] GetAllChildRequestParametersDto childRequestParametersDto)
        {
            var result = await membershipRequestWebService.GetAllChildRequestsAsync(childRequestParametersDto);
            return result.GetResponse();
        }
        [HttpGet("GetAllChildRequestsByBatch")]
        public async Task<IResult> GetAllChildRequestsByBatch([FromQuery] GetAllChildRequestParametersDto childRequestParametersDto,int batchId)
        {
            var result = await membershipRequestWebService.GetAllChildRequestsByBatchAsync(childRequestParametersDto,batchId);
            return result.GetResponse();
        }
        [HttpPut("SetBatchRequestsToPending")]
        public async Task<IResult> SetBatchRequestsToPending(int batchId)
        {
            var result = await membershipRequestWebService.SetBatchRequestsToPending(batchId);
            return result.GetResponse();
        }

    }
}
