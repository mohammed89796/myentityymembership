using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.RequestTypeCategoryDtos;
using MyEntityMembershipService.Repositories.GenericRepo;
using System;
using System.Linq;

namespace MyEntityMembershipService.Repositories.Membership.MembershipRequestRepository
{
    public class MembershipRequestRepository : GenericRepository<MembershipRequest>, IMembershipRequestRepository
    {
        private readonly EntityMembershipDbContext context;
        private readonly IMapper _mapper;


        public MembershipRequestRepository(EntityMembershipDbContext context, IMapper mapper) : base(context)
        {
            this.context = context;
            _mapper = mapper;
        }

        public async Task<List<MembershipRequest>> GetAllRequestsByTenantId(int tenantId)
        {
            var membershipRequestsByTenantId = await context.MembershipRequests
                           .Include(m => m.RequestType)
                                .ThenInclude(t => t.RequestTypeCategory)
                           .Include(m => m.PaymentRequests)
                           //.ThenInclude(m => m.Tenant)
                           .Include(m => m.RequestStatusHistories)
                           .Include(m => m.PaymentRequests).ThenInclude(t => t.Transactions)
                            .Include(m => m.SubMembers)
                            .Where(m => m.TenantId == tenantId && m.Status != RequestStatus.Draft && m.SuperMembershipRequestId == null)
                           .OrderByDescending(m => m.Id)
            .ToListAsync();

            return membershipRequestsByTenantId;

        }

        public async Task<GetAllRequestsGroupedResponseDto> GetAllRequestsGrouped(int tenantId)
        {
            // Fetch the data from the database
            var membershipRequests = await GetAllRequestsByTenantId(tenantId);

            // Calculate the total number of requests
            var totalRequests = membershipRequests.Count;

            // Group by RequestTypeCategory first, then by RequestType
            var groupedRequests = membershipRequests
                .GroupBy(r => new { r.RequestType.RequestTypeCategory.Id, r.RequestType.RequestTypeCategory.Name })
                .Select(g => new RequestTypeCategoryDto
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    RequestTypes = g.GroupBy(r => new { r.RequestType.Id, r.RequestType.Name })
                                    .Select(rt => new GetAllRequestesDto
                                    {
                                        RequestTypeId = rt.Key.Id,
                                        RequestTypeName = rt.Key.Name,
                                        GroupTotalRequests = rt.Count(),
                                        StatusGroups = rt.GroupBy(r => r.Status)
                                                         .Select(sg => new RequestStatusGroupDto
                                                         {
                                                             Status = sg.Key,
                                                             StatusGroupTotalRequests = sg.Count(),
                                                         }).ToList()
                                    }).ToList()
                })
                .ToList();

            // Prepare the response DTO
            var responseDto = new GetAllRequestsGroupedResponseDto
            {
                TotalRequests = totalRequests,
                GroupedRequests = groupedRequests
            };

            return responseDto;
        }
        public async Task<List<MembershipRequestDto>> GetRequestsByTypeID(int tenantId, int requestTypeId)
        {
            var allMembershipRequests = await GetAllRequestsByTenantId(tenantId);
            var membershipRequestsByTypeID = allMembershipRequests
                           .Where(m => m.RequestTypeId == requestTypeId)
                           .OrderByDescending(m => m.Id)
                           .ToList();
            var membershipRequestsByTypeIDDto = _mapper.Map<List<MembershipRequestDto>>(membershipRequestsByTypeID);



            return membershipRequestsByTypeIDDto;
        }

        public async Task<List<MembershipRequest>> GetUserMembershipRequests(string userId)
            => await context.MembershipRequests.Where(request => request.UserId == userId && !request.SuperMembershipRequestId.HasValue).OrderByDescending(request => request.CreationDate).ToListAsync();

        public async Task<List<MembershipRequest>> GetMembershipRequestsByName(string userId, string name)
            => await context.MembershipRequests.Where(request => request.FirstName.Contains(name.Trim().ToLower()) && request.UserId == userId && !request.SuperMembershipRequestId.HasValue).OrderByDescending(request => request.CreationDate).ToListAsync();

        public async Task<MembershipRequest> GetMembershipRequestsById(int tenantId, int requestId)
        {
            var membershipRequest = await context.MembershipRequests
                .Where(request => request.TenantId == tenantId && request.Id == requestId)
                    .Include(m => m.Membership) // Include Membership directly
                    .Include(m => m.MemberType) // Include Membership directly

                .Include(m => m.FormUserResponse)
                    .ThenInclude(m => m.FieldsResponse)
                        .ThenInclude(m => m.Field)
                .Include(m => m.FormUserResponse)
                    .ThenInclude(m => m.Membership)
                .Include(m => m.FormUserResponse)
                    .ThenInclude(m => m.SubMemberMembership)
                     .Include(m => m.FormUserResponse)
                    .ThenInclude(m => m.SubMemberMembership)
                .Include(m => m.FormUserResponse)
                    .ThenInclude(m => m.Form)
                .Include(m => m.RequestStatusHistories)
                .Include(m => m.RequestType)
                    .ThenInclude(t => t.RequestTypeCategory)
                .Include(m => m.PaymentRequests)
                    .ThenInclude(t => t.Transactions)
                .Include(m => m.SubMembers)
                    .ThenInclude(s => s.Membership)
                .Include(m => m.SubMembers)
                    .ThenInclude(s => s.MemberType) // Include MemberType inside SubMembers
                .Include(m => m.SubMembers)
                    .ThenInclude(s => s.FormUserResponse) // Include FormUserResponse for each SubMember
                        .ThenInclude(f => f.FieldsResponse)
                            .ThenInclude(fr => fr.Field) // Nested include for FieldsResponse.Field within SubMember's FormUserResponse
                .Include(m => m.SubMembers)
                    .ThenInclude(s => s.FormUserResponse)
                        .ThenInclude(f => f.Membership) // Include Membership in SubMember's FormUserResponse

                         .Include(m => m.SubMembers)
                    .ThenInclude(s => s.FormUserResponse)
                        .ThenInclude(f => f.Form) // Include form
                           .Include(m => m.SubMembers)
                    .ThenInclude(s => s.RequestStatusHistories)
                .FirstOrDefaultAsync();

            return membershipRequest;
        }

        public async Task<List<MembershipRequestComment>> GetMembershipRequestComments(int Id)
            => await context.MembershipRequestComments.Where(comment => comment.MembershipRequestId == Id).ToListAsync();


        public async Task<List<Data.Membership.Entities.PaymentRequest>> GetMembershipRequestPayments(int Id)
            => await context.PaymentRequests.Where(payment => payment.MembershipRequestId == Id).ToListAsync();

        public async Task<List<MembershipRequest>> GetMembershipSubmembers(int Id)
            => await context.MembershipRequests.Where(submember => submember.SuperMembershipRequestId == Id).ToListAsync();
        public async Task<List<MembershipRequest>> GeMembershipRequestByPackageId(int Id)
            => await context.MembershipRequests.Where(request => request.PackageId == Id && request.SuperMembershipRequestId == null).Include(m => m.Membership).Include(r => r.RequestType).OrderByDescending(m => m.CreationDate).ToListAsync();

        public async Task<List<MembershipRequest>> GetRequestsOfMembershipByType(int requestTypeId, int membershipId)
            => await context.MembershipRequests.Where(request => request.RequestTypeId == requestTypeId && request.MembershipId == membershipId && request.Status == RequestStatus.RequestResolved).ToListAsync();

        public async Task<List<MembershipRequest>> GetResolvedRequestsOfMembership(int membershipId)
            => await context.MembershipRequests.Where(request => request.MembershipId == membershipId && request.Status == RequestStatus.RequestResolved).ToListAsync();

        public async Task<List<MembershipRequestBackgroundJobs>> GetMembershipRequestJobs(int requestId)
            => await context.MembershipRequestBackgroundJobs.Where(item => item.MembershipRequestId == requestId).ToListAsync();

        public async Task<MembershipRequest> GetMembershipRequestWithStatusHistory(int requestId)
            => await context.MembershipRequests.Where(request => request.Id == requestId)
                                                .Include(request => request.MemberType)
                                                .Include(request => request.SubMembers)
                                                .ThenInclude(submember => submember.FormUserResponse)
                                                .ThenInclude(response => response.FieldsResponse)
                                                .ThenInclude(response => response.Field)
                                                .ThenInclude(field => field.FieldType)
                                                .Include(request => request.SubMembers) // Include SubMembers again to chain further includes
                                                .ThenInclude(submember => submember.MemberType) // Include MemberType for SubMembers
                                               .Include(request => request.RequestStatusHistories).FirstOrDefaultAsync();

        public async Task<PaginatedFedEntityRequestsList> GetFedEntityRequestsByTenantId(int tenantId, int typeId, FedEntityRequestFilterDto filter)
        {
            var query = context.MembershipRequests
            .Include(m => m.FormUserResponse)
                .ThenInclude(fur => fur.FieldsResponse)
                .Include(m => m.NewPaymentRequests)
                .Include(m => m.NewPaymentRequests)
            .Where(m => m.TenantId == tenantId && m.RequestTypeId == typeId && m.Status != RequestStatus.Draft);

            if (filter.FromDate.HasValue)
            {
                var fromDateTime = filter.FromDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(m => m.CreationDate >= fromDateTime);
            }

            if (filter.ToDate.HasValue)
            {
                var toDateTime = filter.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
                query = query.Where(m => m.CreationDate <= toDateTime);
            }

            if (filter.RequestStatus.HasValue)
            {
                query = query.Where(m => (int)m.Status == filter.RequestStatus.Value);
            }

            if (filter.BeltLevel is not null)
            {
                query = query.Where(m => m.BeltName == filter.BeltLevel);
            }

            if (filter.PaymentStatus.HasValue)
            {
                query = query.Where(m =>
                    m.NewPaymentRequests.Any(p => (int)p.PaymentStatus == filter.PaymentStatus.Value));
            }
            if (filter.BranchPaymentStatus.HasValue)
            {
                query = query.Where(m =>
                    m.NewPaymentRequests.Any(p => (int)p.BranchAdminStatus == filter.BranchPaymentStatus.Value));
            }
            if (filter.BranchRequestStatus.HasValue)
            {
                query = query.Where(m => (int)m.BranchAdminStatus == filter.BranchRequestStatus.Value);
            }


            if (!string.IsNullOrWhiteSpace(filter.SearchData))
            {
                var search = filter.SearchData.Trim().ToLower();

                query = query.Where(m =>
                    (!string.IsNullOrEmpty(m.NationalId) && m.NationalId.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(m.FirstName) && (m.FirstName + " " + m.LastName).ToLower().Contains(search)) ||
                    (!string.IsNullOrWhiteSpace(m.FileNumber) && m.FileNumber.ToLower().Contains(search))
                );
            }


            int totalCount = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var result = await query
                .OrderByDescending(m => m.Id)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(m => new GetFedEntityRequestsDto
                {
                    Id = m.Id,
                    Name = m.FirstName + " " + m.SecondName + " " + m.MiddleName + " " + m.LastName,
                    FileNumber = !string.IsNullOrEmpty(m.FileNumber) ? m.FileNumber : "N/A",
                    DateOfSubmission = m.CreationDate.HasValue ? DateOnly.FromDateTime(m.CreationDate.Value) : default,
                    NationalID = !string.IsNullOrEmpty(m.NationalId) ? m.NationalId : "-",
                    DateOfBirth = m.BirthDate.HasValue ? DateOnly.FromDateTime(m.BirthDate.Value) : default,
                    BeltLevel = !string.IsNullOrEmpty(m.BeltName) ? m.BeltName : "N/A",
                    PaymentRequestID = m.NewPaymentRequests != null && m.NewPaymentRequests.Any() ? (m.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().PaymentBatchId ?? 0) : 0,
                    DateOfAcquisition = !string.IsNullOrEmpty(m.DateOfAcquisition) ? m.DateOfAcquisition : "N/A",
                    RequestStatus = (int)m.Status,
                    PaymetStatus = m.NewPaymentRequests != null && m.NewPaymentRequests.Any() ? (int)m.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().PaymentStatus : 0,
                    BranchAdminStatus = m.BranchAdminStatus,
                    PaymentBranchAdminStatus = m.NewPaymentRequests != null && m.NewPaymentRequests.Any()
                    ? m.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault() != null
                        ? m.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().BranchAdminStatus
                        : BranchAdminStatus.Pending
                    : BranchAdminStatus.Pending


                })
                .ToListAsync();

            return new PaginatedFedEntityRequestsList
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                Data = result
            };
        }
        public async Task<PaginatedFedRequestsList> GetFedRequestsByTenantId(int tenantId, List<int> areaIds, int typeId, FedRequestFilterDto filter)
        {
            IQueryable<MembershipRequest> query;

            query = context.MembershipRequests
        .Include(m => m.FormUserResponse)
            .ThenInclude(fur => fur.FieldsResponse)
         .Include(m => m.Tenant)
            .ThenInclude(fur => fur.Area)
            .Include(m => m.NewPaymentRequests)
            .Where(m => m.SuperTenantId == tenantId && m.Status != RequestStatus.Draft && m.RequestTypeId == typeId && (!areaIds.Any() || areaIds.Contains(m.Tenant.AreaId ?? 0)));


            if (filter.Date.HasValue)
            {
                var filterDate = filter.Date.Value;
                query = query.Where(m => m.CreationDate.HasValue && DateOnly.FromDateTime(m.CreationDate.Value) == filterDate);
            }

            if (filter.RequestStatus.HasValue)
            {
                query = query.Where(m => (int)m.Status == filter.RequestStatus.Value);
            }

            if (filter.BeltLevel is not null)
            {
                query = query.Where(m => m.BeltName == filter.BeltLevel);
            }

            if (filter.PaymentStatus.HasValue)
            {
                query = query.Where(m =>
                    m.NewPaymentRequests.Any(p => (int)p.PaymentStatus == filter.PaymentStatus.Value));
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchData))
            {
                var search = filter.SearchData.Trim().ToLower();

                query = query.Where(m =>
                    (!string.IsNullOrEmpty(m.NationalId) && m.NationalId.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(m.FirstName) && (m.FirstName + " " + m.LastName).ToLower().Contains(search)) ||
                    (!string.IsNullOrWhiteSpace(m.FileNumber) && m.FileNumber.ToLower().Contains(search))
                );
            }

            int totalCount = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var result = await query
                .OrderByDescending(m => m.Id)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(m => new GetFedRequestsDto
                {
                    Id = m.Id,
                    Name = m.FirstName + " " + m.SecondName + " " + m.MiddleName + " " + m.LastName,
                    Entity = m.Batch.TenantName,
                    Branch = m.Tenant.Area != null ? m.Tenant.Area.Name : "Unknown Name",
                    FileNumber = !string.IsNullOrEmpty(m.FileNumber) ? m.FileNumber : "N/A",
                    NationalID = !string.IsNullOrEmpty(m.NationalId) ? m.NationalId : "-",
                    DateOfBirth = m.BirthDate.HasValue ? DateOnly.FromDateTime(m.BirthDate.Value) : default,
                    BeltLevel = !string.IsNullOrEmpty(m.BeltName) ? m.BeltName : "N/A",
                    DateOfAcquisition = !string.IsNullOrEmpty(m.DateOfAcquisition) ? m.DateOfAcquisition : "N/A",
                    RequestStatus = (int)m.Status,
                    PaymentStatus = m.NewPaymentRequests != null && m.NewPaymentRequests.Any() ? (int)m.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().PaymentStatus : 0,
                    BranchAdminStatus = m.BranchAdminStatus,
                    PaymentBranchAdminStatus = m.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().BranchAdminStatus,
                })
                .ToListAsync();

            return new PaginatedFedRequestsList
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                TotalCount = totalCount,
                Data = result
            };
        }
        public async Task<PaginatedFedEntityRequestsList> GetFedEntityRequestsByBatchId(int batchID, int typeId, FedEntityRequestByBatchFilterDto filter)
        {
            var query = context.MembershipRequests
            .Include(m => m.FormUserResponse)
                .ThenInclude(fur => fur.FieldsResponse)
                .Include(m => m.NewPaymentRequests)
            .Where(m => m.BatchId == batchID && m.Status != RequestStatus.Draft && m.RequestTypeId == typeId);

            if (filter.Date.HasValue)
            {
                var fromDateTime = filter.Date.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(m => m.CreationDate == fromDateTime);
            }

            if (filter.RequestStatus.HasValue)
            {
                query = query.Where(m => (int)m.Status == filter.RequestStatus.Value);
            }

            if (filter.BeltLevel is not null)
            {
                query = query.Where(m => m.BeltName == filter.BeltLevel);
            }

            if (filter.PaymentStatus.HasValue)
            {
                query = query.Where(m =>
                    m.NewPaymentRequests.Any(p => (int)p.PaymentStatus == filter.PaymentStatus.Value));
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchData))
            {
                var search = filter.SearchData.Trim().ToLower();

                query = query.Where(m =>
                    (!string.IsNullOrEmpty(m.NationalId) && m.NationalId.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(m.FirstName) && (m.FirstName + " " + m.LastName).ToLower().Contains(search)) ||
                    (!string.IsNullOrWhiteSpace(m.FileNumber) && m.FileNumber.ToLower().Contains(search))
                );
            }

            int totalCount = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var result = await query
                .OrderByDescending(m => m.Id)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(m => new GetFedEntityRequestsDto
                {
                    Id = m.Id,
                    Name = m.FirstName + " " + m.SecondName + " " + m.MiddleName + " " + m.LastName,
                    FileNumber = !string.IsNullOrEmpty(m.FileNumber) ? m.FileNumber : "N/A",
                    DateOfSubmission = m.CreationDate.HasValue ? DateOnly.FromDateTime(m.CreationDate.Value) : default,
                    NationalID = !string.IsNullOrEmpty(m.NationalId) ? m.NationalId : "-",
                    DateOfBirth = m.BirthDate.HasValue ? DateOnly.FromDateTime(m.BirthDate.Value) : default,
                    BeltLevel = !string.IsNullOrEmpty(m.BeltName) ? m.BeltName : "N/A",
                    PaymentRequestID = m.NewPaymentRequests != null && m.NewPaymentRequests.Any() ? (m.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().PaymentBatchId ?? 0) : 0,
                    DateOfAcquisition = !string.IsNullOrEmpty(m.DateOfAcquisition) ? m.DateOfAcquisition : "N/A",
                    RequestStatus = (int)m.Status,
                    PaymetStatus = m.NewPaymentRequests != null && m.NewPaymentRequests.Any() ? (int)m.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().PaymentStatus : 0,
                    BranchAdminStatus = m.BranchAdminStatus,
                    PaymentBranchAdminStatus = m.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().BranchAdminStatus,
                })
                .ToListAsync();

            return new PaginatedFedEntityRequestsList
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                Data = result
            };
        }
        public async Task<PaginatedFederationEntityBatchList> GetFedEntityBatchesByTenantId(int tenantID, int typeId, FedEntityBatchFilterDto filter)
        {
            var query = context.Batches
                .Include(b => b.MembershipRequests.Where(r => r.RequestTypeId == typeId))
                .Where(m => m.TenantId == tenantID && m.Status != BatchStatus.Draft && m.RequestTypeId == typeId);

            if (filter.Date.HasValue)
            {
                var fromDateTime = filter.Date.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(m => m.CreationDate == fromDateTime);
            }

            if (filter.Date.HasValue)
            {
                var fromDateTime = filter.Date.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(m => m.CreationDate.HasValue && m.CreationDate.Value.Date == fromDateTime.Date);
            }

            if (filter.SearchData is not null)
            {
                var search = filter.SearchData.Trim().ToLower();
                query = query.Where(sd => sd.BatchNumber.Contains(filter.SearchData));
            }

            int totalCount = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var result = await query
                .OrderByDescending(m => m.Id)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(m => new GetFederationEntityBatchDto
                {
                    Id = m.Id,
                    BatchNumber = m.BatchNumber,
                    DateOfSubmission = m.CreationDate.HasValue ? DateOnly.FromDateTime(m.CreationDate.Value) : default,
                    TotalAthletes = m.MembershipRequests.Count,
                    ApprovedAthletes = m.MembershipRequests.Count(r => r.Status == RequestStatus.RequestResolved),
                    RejectedAthletes = m.MembershipRequests.Count(r => r.Status == RequestStatus.Rejected),
                    PendingAthletes = m.MembershipRequests.Count(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.PendingPayment),

                })
                .ToListAsync();


            return new PaginatedFederationEntityBatchList
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                Data = result
            };
        }

        public async Task<PaginatedFederationEntityBatchList> GetFedEntityDraftBatchesByTenantId(int tenantID, int typeId, FedEntityBatchFilterDto filter)
        {
            var query = context.Batches
                .Include(b => b.MembershipRequests)
                .Where(m => m.TenantId == tenantID && m.Status == BatchStatus.Draft && m.RequestTypeId == typeId);

            if (filter.Date.HasValue)
            {
                var fromDateTime = filter.Date.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(m => m.CreationDate == fromDateTime);
            }
            if (filter.Date.HasValue)
            {
                var fromDateTime = filter.Date.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(m => m.CreationDate.HasValue && m.CreationDate.Value.Date == fromDateTime.Date);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchData))
            {
                var search = filter.SearchData.Trim().ToLower();
                query = query.Where(sd => sd.BatchNumber.Contains(filter.SearchData));
            }

            int totalCount = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var result = await query
                .OrderByDescending(m => m.Id)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(m => new GetFederationEntityBatchDto
                {
                    Id = m.Id,
                    BatchNumber = m.BatchNumber,
                    DateOfSubmission = m.CreationDate.HasValue ? DateOnly.FromDateTime(m.CreationDate.Value) : default,
                    TotalAthletes = m.MembershipRequests.Count,
                    ApprovedAthletes = m.MembershipRequests.Count(r => r.Status == RequestStatus.RequestResolved),
                    RejectedAthletes = m.MembershipRequests.Count(r => r.Status == RequestStatus.Rejected),
                    PendingAthletes = m.MembershipRequests.Count(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.PendingPayment),

                })
                .ToListAsync();


            return new PaginatedFederationEntityBatchList
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                Data = result
            };
        }
        public async Task<PaginatedFederationBatchList> GetFedBatchesByTenantId(int tenantID, List<int> areaIds, int typeId, FedBatchFilterDto filter)
        {
            IQueryable<Batch> query;

            query = context.Batches
                    .Include(b => b.MembershipRequests)
                    .ThenInclude(b => b.Tenant)
                    .ThenInclude(t => t.Area)
                    .Where(m => m.SuperTenentId == tenantID && m.Status != BatchStatus.Draft && m.RequestTypeId == typeId && (!areaIds.Any() || areaIds.Contains(m.Tenant.AreaId ?? 0)));
            if (filter.Date.HasValue)
            {
                var fromDateTime = filter.Date.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(m => m.CreationDate.HasValue && m.CreationDate.Value.Date == fromDateTime.Date);
            }

            if (filter.TenentID > 0)
            {
                query = query.Where(m => m.TenantId == filter.TenentID);
            }
            if (filter.AreaID > 0)
            {
                query = query.Where(m => m.Tenant != null && m.Tenant.AreaId == filter.AreaID);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchData))
            {
                var search = filter.SearchData.Trim().ToLower();

                query = query.Where(m =>
                    (!string.IsNullOrEmpty(m.BatchNumber) && m.BatchNumber.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(m.TenantName) && m.TenantName.ToLower().Contains(search)) ||
                    (m.Tenant != null && m.Tenant.Area != null && m.Tenant.Area.Name.ToLower().Contains(search))
                );
            }


            int totalCount = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var rawData = await query
                .OrderByDescending(m => m.Id)
                .Skip(skip)
                .Take(filter.PageSize)
                .ToListAsync();

            var data = rawData.Select(m => new GetFederationBatchDto
            {
                Id = m.Id,
                BatchNumber = m.BatchNumber,
                EntityName = m.TenantName,
                Branch = m.MembershipRequests.Where(r => r.Tenant != null).Select(r => r.Tenant.Area.Name).FirstOrDefault(),
                DateOfSubmission = m.CreationDate.HasValue ? DateOnly.FromDateTime(m.CreationDate.Value) : default,
                TotalAthletes = m.MembershipRequests.Count,
                ApprovedAthletes = m.MembershipRequests.Count(r => r.Status == RequestStatus.RequestResolved),
                RejectedAthletes = m.MembershipRequests.Count(r => r.Status == RequestStatus.Rejected),
                PendingAthletes = m.MembershipRequests.Count(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.PendingPayment),
            }).ToList();

            return new PaginatedFederationBatchList
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                TotalCount = totalCount,
                Data = data
            };
        }
        public async Task<MembershipRequestDetailsDto?> GetMembershipRequestDetails(int requestId)
        {
            var fieldCategoryMap = new Dictionary<string, string>
            {
                ["Gender"] = "Athlete Details",
                ["BirthDate"] = "Athlete Details",
                ["NationalId"] = "Athlete Details",
                ["BeltName"] = "Athlete Details",
                ["DateOfAcquisition"] = "Athlete Details",
                ["MilitaryStatus"] = "Athlete Details",
                ["PassportNumber"] = "Athlete Details",
                ["DateOfIssuance"] = "Athlete Details",
                ["DateOfExpiry"] = "Athlete Details",

                ["HomeNumber"] = "Contact Information",
                ["PhoneNumber"] = "Contact Information",
                ["Email"] = "Contact Information",
                ["Address"] = "Contact Information",
                ["School/Work Entity"] = "Contact Information",


                ["ProfileImage"] = "Documents Submitted",
                ["BirthCertificateOrNationalID"] = "Documents Submitted",
                ["BeltCertificate"] = "Documents Submitted",
                ["TransferRequestDocument"] = "Documents Submitted",
                ["loanRequestDocument"] = "Documents Submitted"
            };

            var request = await context.MembershipRequests
                .Include(r => r.FormUserResponse)
                    .ThenInclude(fr => fr.FieldsResponse)
                        .ThenInclude(f => f.Field)
                    .Include(r => r.NewPaymentRequests)
                      .Include(rt => rt.Tenant)
                        .ThenInclude(rta => rta.Area)

                        .Include(rft => rft.FromTenant)
                        .ThenInclude(rfta => rfta.Area)
                      .Include(m => m.Membership)
                        .ThenInclude(mm => mm.MembershipRequests)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request?.FormUserResponse?.FieldsResponse == null)
                return null;

            var validFields = request.FormUserResponse.FieldsResponse
                .Where(fr => fr?.Field != null && fieldCategoryMap.ContainsKey(fr.Field.MapField));

            var categorizedGroups = validFields
                .Where(fr => fieldCategoryMap[fr.Field.MapField] != "Top")
                .GroupBy(fr => fieldCategoryMap[fr.Field.MapField])
                .Select(group => new CategoryGroupDto
                {
                    Category = group.Key,
                    Fields = group.Select(fr => new DetailedFieldResponseDto
                    {
                        Title = fr.Field.Title ?? "Untitled",
                        Value = fr.Response ?? ""
                    }).ToList()
                })
                .ToList();

            var topFields = validFields
                .Where(fr => fieldCategoryMap[fr.Field.MapField] == "Top")
                .Select(fr => new DetailedFieldResponseDto
                {
                    Title = fr.Field.Title ?? "Untitled",
                    Value = fr.Response ?? ""
                })
                .ToList();

            if (topFields.Any())
            {
                categorizedGroups.Insert(0, new CategoryGroupDto
                {
                    Category = "",
                    Fields = topFields
                });
            }
            var membership = request.Membership;
            string? transferredTo = null;
            string? transferredFrom = null;

            if (membership != null && request.RequestTypeId !=6)
            {
                var membershipFieldMap = new Dictionary<string, (string Category, string Title, string? Value)>
                {
                    // Athlete Details
                    ["Gender"] = ("Athlete Details", "Gender", membership.Gender?.ToString()),
                    ["BirthDate"] = ("Athlete Details", "Date of Birth", membership.BirthDate?.ToString("yyyy-MM-dd")),
                    ["NationalId"] = ("Athlete Details", "National ID", membership.NationalId),
                    ["BeltLevel"] = ("Athlete Details", "Level", membership.BeltName),
                    ["AcquisitionDate"] = ("Athlete Details", "Date of Acquisition", membership.DateOfAcquisition),
                    ["MilitaryStatus"] = ("Athlete Details", "Military Status", membership.MilitaryStatus),
                    ["PassportNumber"] = ("Athlete Details", "Passport Number", membership.PassportNumber),
                    ["DateOfIssuance"] = ("Athlete Details", "Date of Issuance", membership.DateOfIssuance?.ToString("yyyy-MM-dd")),
                    ["DateOfExpiry"] = ("Athlete Details", "Date of Expiry", membership.DateOfExpiry?.ToString("yyyy-MM-dd")),

                    // Contact Information
                    ["HomeNumber"] = ("Contact Information", "Home Number", membership.HomeNumber),
                    ["PhoneNumber"] = ("Contact Information", "Phone Number", membership.PhoneNumber),
                    ["Address"] = ("Contact Information", "Address", membership.Address),
                    ["Email"] = ("Contact Information", "Email Address", membership.Email),
                    ["SchoolWorkEntity"] = ("Contact Information", "School/Work Entity", membership.SchoolOrWorkEntity),

                    // Documents Submitted
                    ["BeltCertificate"] = ("Documents Submitted", "Belt Certificate", membership.BeltCertificate),
                    ["UserPhoto"] = ("Documents Submitted", "Profile Image", membership.UserPhoto),
                    ["NationalIDPhoto"] = ("Documents Submitted", "Birth Certificate or National ID", membership.NationalIDPhoto),

                };

               

            foreach (var kvp in membershipFieldMap)
                {
                    var (category, title, value) = kvp.Value;

                    if (string.IsNullOrWhiteSpace(value))
                        continue;
                    var existingCategory = categorizedGroups.FirstOrDefault(g => g.Category == category);

                    if (existingCategory == null)
                    {
                        existingCategory = new CategoryGroupDto
                        {
                            Category = category,
                            Fields = new List<DetailedFieldResponseDto>()
                        };
                        categorizedGroups.Add(existingCategory);
                    }

                    existingCategory.Fields.Add(new DetailedFieldResponseDto
                    {
                        Title = title,
                        Value = value
                    });

                }
                transferredTo = $"{request?.Tenant?.Area?.Name ?? "Unknown Branch"} - {request?.Tenant?.Name ?? "Unknown Entity"}";
                transferredFrom = $"{request?.FromTenant?.Area?.Name ?? "Unknown Branch"} - {request?.FromTenant?.Name ?? "Unknown Entity"}";

            }

            var categorizedGroupsList = categorizedGroups.Select(group => new CategoryGroupDto
            {
                Category = group.Category,
                Fields = group.Fields
            }).ToList();

            return new MembershipRequestDetailsDto
            {
                RequestId = request.Id,
                Name =request.FirstName + " " + request.SecondName + " " + request.MiddleName + " " + request.LastName,
                ProfileImageURL = string.IsNullOrWhiteSpace(request?.UserPhoto) ? "N/A" : request.UserPhoto,
                FileNumber = string.IsNullOrWhiteSpace(request?.FileNumber) ? "N/A" : request.FileNumber,
                RequestStatus = request?.Status ?? RequestStatus.Pending,
                PaymentStatus = request?.NewPaymentRequests?.OrderBy(r => r.Id).LastOrDefault()?.PaymentStatus ?? PaymentStatus.Pending,
                PaymentRequestID = request?.NewPaymentRequests?.OrderBy(r => r.Id).LastOrDefault().PaymentBatchId ?? 0,
                BranchAdminStatus = request?.BranchAdminStatus ?? BranchAdminStatus.Pending,
                PaymentBranchAdminStatus = request?.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().BranchAdminStatus ?? BranchAdminStatus.Pending,
                TransferredTo = membership != null ? transferredTo : null,
                TransferredFrom = membership != null ? transferredFrom : null,
                CategorizedResponses = categorizedGroups
            };
        }

        public async Task<List<MembershipRequestDetailsByBatchDto>> GetMembershipRequestsByBatch(int batchId)
        {
            var fieldCategoryMap = new Dictionary<string, string>
            {
                ["Gender"] = "Athlete Details",
                ["BirthDate"] = "Athlete Details",
                ["NationalId"] = "Athlete Details",
                ["BeltName"] = "Athlete Details",
                ["DateOfAcquisition"] = "Athlete Details",
                ["MilitaryStatus"] = "Athlete Details",
                ["PassportNumber"] = "Athlete Details",
                ["DateOfIssuance"] = "Athlete Details",
                ["DateOfExpiry"] = "Athlete Details",

                ["HomeNumber"] = "Contact Information",
                ["PhoneNumber"] = "Contact Information",
                ["Email"] = "Contact Information",
                ["Address"] = "Contact Information",
                ["School/Work Entity"] = "Contact Information",


                ["ProfileImage"] = "Documents Submitted",
                ["BirthCertificateOrNationalID"] = "Documents Submitted",
                ["BeltCertificate"] = "Documents Submitted",
            };

            var requests = await context.MembershipRequests
                .Include(r => r.FormUserResponse)
                    .ThenInclude(fr => fr.FieldsResponse)
                        .ThenInclude(f => f.Field)
                .Where(r => r.BatchId == batchId)
                .ToListAsync();

            var result = new List<MembershipRequestDetailsByBatchDto>();

            foreach (var request in requests)
            {
                if (request?.FormUserResponse?.FieldsResponse == null)
                    continue;

                var validFields = request.FormUserResponse.FieldsResponse
                    .Where(fr => fr?.Field != null && fieldCategoryMap.ContainsKey(fr.Field.MapField));

                var categorizedGroups = validFields
                    .Where(fr => fieldCategoryMap[fr.Field.MapField] != "Top")
                    .GroupBy(fr => fieldCategoryMap[fr.Field.MapField])
                    .Select(group => new CategoryGroupDto
                    {
                        Category = group.Key,
                        Fields = group.Select(fr => new DetailedFieldResponseDto
                        {
                            Title = fr.Field.Title ?? "Untitled",
                            Value = fr.Response ?? ""
                        }).ToList()
                    })
                    .ToList();

                var topFields = validFields
                    .Where(fr => fieldCategoryMap[fr.Field.MapField] == "Top")
                    .Select(fr => new DetailedFieldResponseDto
                    {
                        Title = fr.Field.Title ?? "Untitled",
                        Value = fr.Response ?? ""
                    })
                    .ToList();

                if (topFields.Any())
                {
                    categorizedGroups.Insert(0, new CategoryGroupDto
                    {
                        Category = "",
                        Fields = topFields
                    });
                }

                result.Add(new MembershipRequestDetailsByBatchDto
                {
                    Name =request.FirstName + " " + request.SecondName + " " + request.MiddleName + " " + request.LastName,
                    ProfileImageURL = string.IsNullOrWhiteSpace(request?.UserPhoto) ? "N/A" : request.UserPhoto,
                    FileNumber = string.IsNullOrWhiteSpace(request?.FileNumber) ? "N/A" : request.FileNumber,
                    RequestId = request.Id,
                    CategorizedResponses = categorizedGroups
                });
            }

            return result;
        }

        public async Task<List<MembershipRequestDetailsDto>> GetBatchDetailedViewAsync(int batchId)
        {
            var fieldCategoryMap = new Dictionary<string, string>
            {
                ["Gender"] = "Athlete Details",
                ["BirthDate"] = "Athlete Details",
                ["NationalId"] = "Athlete Details",
                ["CurrenBeltLevel"] = "Athlete Details",
                ["DateOfAcquisition"] = "Athlete Details",
                ["MilitaryStatus"] = "Athlete Details",

                ["HomeNumber"] = "Contact Information",
                ["PhoneNumber"] = "Contact Information",
                ["Email"] = "Contact Information",
                ["Address"] = "Contact Information",
                ["School/Work Entity"] = "Contact Information",
                ["TransferRequestDocument"] = "Documents Submitted",
                ["loanRequestDocument"] = "Documents Submitted",


                ["ProfileImage"] = "Documents Submitted",
                ["BirthCertificateOrNationalID"] = "Documents Submitted",
                ["BeltCertificate"] = "Documents Submitted",
            };
         

            var requests = await context.MembershipRequests
                .Include(r => r.FormUserResponse)
                    .ThenInclude(fr => fr.FieldsResponse)
                        .ThenInclude(f => f.Field)
                        .Include(r => r.NewPaymentRequests)
                        .Include(rt => rt.Tenant)
                        .ThenInclude(rta=>rta.Area)

                        .Include(rft => rft.FromTenant)
                        .ThenInclude(rfta => rfta.Area)
                      

                .Where(r => r.BatchId == batchId)
                .ToListAsync();

            var result = new List<MembershipRequestDetailsDto>();

            foreach (var request in requests)
            {
                if (request?.FormUserResponse?.FieldsResponse == null)
                    continue;

                var validFields = request.FormUserResponse.FieldsResponse
                    .Where(fr => fr?.Field != null && fieldCategoryMap.ContainsKey(fr.Field.MapField));

                var categorizedGroups = validFields
                    .Where(fr => fieldCategoryMap[fr.Field.MapField] != "Top")
                    .GroupBy(fr => fieldCategoryMap[fr.Field.MapField])
                    .Select(group => new CategoryGroupDto
                    {
                        Category = group.Key,
                        Fields = group.Select(fr => new DetailedFieldResponseDto
                        {
                            Title = fr.Field.Title ?? "Untitled",
                            Value = fr.Response ?? ""
                        }).ToList()
                    })
                    .ToList();

                var topFields = validFields
                    .Where(fr => fieldCategoryMap[fr.Field.MapField] == "Top")
                    .Select(fr => new DetailedFieldResponseDto
                    {
                        Title = fr.Field.Title ?? "Untitled",
                        Value = fr.Response ?? ""
                    })
                    .ToList();

                if (topFields.Any())
                {
                    categorizedGroups.Insert(0, new CategoryGroupDto
                    {
                        Category = "",
                        Fields = topFields
                    });
                }

                result.Add(new MembershipRequestDetailsDto
                {
                    RequestId = request.Id,
                    Name = request.FirstName + " " + request.SecondName + " " + request.MiddleName + " " + request.LastName,
                    ProfileImageURL = request.FormUserResponse.FieldsResponse.FirstOrDefault(f => f.Field?.MapField == "UserPhoto")?.Response,
                    FileNumber = request.FormUserResponse.FieldsResponse.FirstOrDefault(f => f.Field?.MapField == "FileNumber")?.Response,
                    PaymentRequestID = request.NewPaymentRequests != null && request.NewPaymentRequests.Any() ? (request.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().PaymentBatchId ?? 0) : 0,
                    RejectionReason = request?.RejectionReason?? null,
                    RequestStatus = request?.Status ?? RequestStatus.Pending,
                    PaymentStatus = request?.NewPaymentRequests?.OrderBy(r => r.Id).LastOrDefault().PaymentStatus ?? PaymentStatus.Pending,
                    BranchAdminStatus = request?.BranchAdminStatus ?? BranchAdminStatus.Pending,
                    PaymentBranchAdminStatus = request?.NewPaymentRequests.OrderBy(r => r.Id).LastOrDefault().BranchAdminStatus ?? BranchAdminStatus.Pending,
                    TransferredTo = $"{request?.Tenant?.Area?.Name ?? "Unknown Branch"} - {request?.Tenant?.Name ?? "Unknown Entity"}",
                    TransferredFrom = $"{request?.FromTenant?.Area?.Name ?? "Unknown Branch"} - {request?.FromTenant?.Name ?? "Unknown Entity"}",

                    CategorizedResponses = categorizedGroups
                }); ;
            }

            return result;
        }
        public async Task<GetFedEntityAnalytics> GetFedEntityMembershipAnalyticsAsync(int tenantID, int typeId)
        {
            var batches = await context.Batches
                .Include(b => b.MembershipRequests.Where(r => r.RequestTypeId == typeId))
                .Where(b => b.TenantId == tenantID&& b.RequestTypeId == typeId && b.Status != BatchStatus.Draft)
                .ToListAsync();

            var totalRequests = batches.SelectMany(b => b.MembershipRequests).ToList();

            var approved = totalRequests.Count(r => r.Status == RequestStatus.RequestResolved);
            var rejected = totalRequests.Count(r => r.Status == RequestStatus.Rejected);
            var pending = totalRequests.Count(r => r.Status == RequestStatus.Pending);

            return new GetFedEntityAnalytics
            {
                NumberOfBatches = batches.Count.ToString(),
                NumberOfRequests = totalRequests.Count.ToString(),
                ApprovedAthletes = approved.ToString(),
                RejectedAthletes = rejected.ToString(),
                PendingAthletes = pending.ToString()
            };
        }
        public async Task<GetFedEntityAnalytics> GetFedMembershipAnalyticsAsync(int tenantID, List<int> areaIds, int typeId)
        {
            List<Batch> batches;

            batches = await context.Batches
               .Include(b => b.MembershipRequests.Where(m => m.Status != RequestStatus.Draft && m.RequestTypeId == typeId && (!areaIds.Any() || areaIds.Contains(m.Tenant.AreaId ?? 0))))
               .Where(b => b.SuperTenentId == tenantID && b.RequestTypeId == typeId && b.Status != BatchStatus.Draft && (!areaIds.Any() || areaIds.Contains(b.Tenant.AreaId ?? 0)))
               .ToListAsync();

            var totalRequests = batches.SelectMany(b => b.MembershipRequests).ToList();

            var approved = totalRequests.Count(r => r.Status == RequestStatus.RequestResolved);
            var rejected = totalRequests.Count(r => r.Status == RequestStatus.Rejected);
            var pending = totalRequests.Count(r => r.Status == RequestStatus.Pending);

            return new GetFedEntityAnalytics
            {
                NumberOfBatches = batches.Count.ToString(),
                NumberOfRequests = totalRequests.Count.ToString(),
                ApprovedAthletes = approved.ToString(),
                RejectedAthletes = rejected.ToString(),
                PendingAthletes = pending.ToString()
            };
        }
        public async Task<GetBatchAnalytics> GetBatchAnalyticsAsync(int batchId, int typeId)
        {
            var batch = await context.Batches
                .Include(b => b.MembershipRequests.Where(r => r.RequestTypeId == typeId))
                    .ThenInclude(mr => mr.RequestType)
                        .Include(b => b.Tenant)
                        .ThenInclude(Tenant => Tenant.Area)
                        .Include(t => t.Tenant)
                        .ThenInclude(t => t.FedrationEntityType)

                .Where(b => b.Id == batchId)
                .FirstOrDefaultAsync();

            var totalRequests = batch.MembershipRequests.Count();
            var approved = batch.MembershipRequests.Count(r => r.Status == RequestStatus.RequestResolved);
            var rejected = batch.MembershipRequests.Count(r => r.Status == RequestStatus.Rejected);
            var pending = batch.MembershipRequests.Count(r => r.Status == RequestStatus.Pending);



            return new GetBatchAnalytics
            {
                BatchNumber = batch.BatchNumber,
                DateOfSubmission = batch.CreationDate.HasValue ? DateOnly.FromDateTime(batch.CreationDate.Value) : (DateOnly?)null,
                TotalRequests = totalRequests.ToString(),
                ApprovedAthletes = approved.ToString(),
                RejectedAthletes = rejected.ToString(),
                PendingAthletes = pending.ToString(),
                EntityType = batch.Tenant?.FedrationEntityType?.Name ?? "Unknown Type",
                BranchName = batch.Tenant?.Area?.Name ?? "Unknown Area"
            };

        }

    }
}
