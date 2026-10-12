using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.DTOs.Membership.FederationDtos;
using MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos;
using MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos.FedrationEntitiesPaymentRequests;
using MyEntityMembershipService.DTOs.Membership.UserDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.Services.Membership.FormServ;
using MyEntityMembershipService.Services.Membership.PublishService;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MyEntityMembershipService.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFetchService fetchService;
        private readonly IFormService _formService;
        private readonly IBackgroundJobClient _backgroundJobClient;


        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IFetchService fetchService, IFormService formService, IBackgroundJobClient backgroundJobClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            this.fetchService = fetchService;
            _formService = formService;
            _backgroundJobClient = backgroundJobClient;

        }

        public async Task<Response<object>> CreatePaymentRequestAsync(AddPaymentRequestDto dto)
        {

            try
            {
                Log.Information("Starting CreatePaymentRequestAsync | BatchId: {BatchId}, EntityId: {EntityId}, RequestsCount: {Count}", dto.BatchId, dto.EntityId, dto.MemberRequestsIds?.Count ?? 0);

                var validationMessage = ValidateOnCreatePaymentRequest(dto);


                if (!string.IsNullOrEmpty(validationMessage))
                {
                    Log.Warning("Validation Failed | Message: {Message}", validationMessage);

                    return new ResponseHandler().BadRequest<object>(validationMessage);
                }

                bool isexists = await _unitOfWork.Repository<NewPaymentRequest>().IsExistsAsync(pr => pr.BatchId == dto.BatchId && dto.MemberRequestsIds.Contains(pr.MembershipRequestId));

                if (isexists)
                {
                    Log.Warning("Duplicate Payment Request | BatchId: {BatchId}", dto.BatchId);

                    return new ResponseHandler().BadRequest<object>("Payment request already exists for the selected membership requests");
                }
                Log.Information("Fetching batch with included membership and payment data | BatchId: {BatchId}", dto.BatchId);

                var batchSpecifications = new GeneralSpecifications<Batch>(b => b.Id == dto.BatchId)
                {
                    Includes = new List<Func<IQueryable<Batch>, IQueryable<Batch>>>
                        {
                         IQ=> IQ.Include(b => b.MembershipRequests.Where(mr=>dto.MemberRequestsIds.Contains(mr.Id)))

                         .Include(b=>b.PaymentBatches.Where(pb=>dto.BatchId==pb.BatchId))

                         .ThenInclude(pb=>pb.NewPaymentRequests.Where(pr=>dto.MemberRequestsIds.Contains(pr.MembershipRequestId)))


                        }

                };



                var batch = await _unitOfWork.Repository<Batch>().GetWithSpecifications(batchSpecifications);
                if (batch == null)
                {
                    Log.Warning("Batch Not Found | BatchId: {BatchId}", dto.BatchId);
                    return new ResponseHandler().BadRequest<object>("Batch not found");
                }
                batch.Status = BatchStatus.Submited;
                Log.Information("Batch status set to Submited | BatchId: {BatchId}", batch.Id);

                List<string> invoiceUrls = new List<string>();

                if (dto.Invoice != null)
                {
                    Log.Information("Uploading {Count} invoice files to container", dto.Invoice.Count);

                    for (int i = 0; i < dto.Invoice.Count; i++)
                    {
                        invoiceUrls.Add(await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", dto.Invoice[i]));
                    }


                }
                Log.Information("Fetching Entity Payment Summary...");
                var entityPaymentSummary = await EntityPaymentSummary(dto.MemberRequestsIds);

                if (!entityPaymentSummary.Succeeded)
                {
                    Log.Warning("EntityPaymentSummary Failed | Message: {Message}", entityPaymentSummary.Message);

                    return new ResponseHandler().BadRequest<object>(entityPaymentSummary.Message);
                }

                await _unitOfWork.Repository<PaymentBatch>().Add(new PaymentBatch
                {
                    BatchId = dto.BatchId,
                    EntityId = dto.EntityId,
                    EntityName = batch.TenantName,
                    SuperEntityName = batch.SuperEntityName,
                    SuperEntityId = batch.SuperTenentId,
                    InvoiceUrl = invoiceUrls,
                    PaymentStatus = PaymentStatus.PendingConfirmation,
                    TotalAmountDue = entityPaymentSummary.Data.TotalAmountDue.HasValue ? entityPaymentSummary.Data.TotalAmountDue.Value : 0,
                    AmountPaid = dto.AmountPaid,
                    PaymentType = dto.PaymentType,
                    OperationType = dto.OperationType,
                    NewPaymentRequests =

                     batch.MembershipRequests.Select(mr => new NewPaymentRequest
                     {
                         MembershipRequestId = mr.Id,
                         PaymentStatus = PaymentStatus.PendingConfirmation,
                         BatchId = dto.BatchId,
                         EntityId = dto.EntityId,
                         EntityName = batch.TenantName,
                         SuperEntityId = batch.SuperTenentId,
                         SuperEntityName = batch.SuperEntityName,
                         OperationType = dto.OperationType,

                     }).ToList()
                });

                var memberRequestsToUpdate = batch.MembershipRequests
                  .Where(mr => dto.MemberRequestsIds.Contains(mr.Id))
                  .ToList();

                foreach (var mr in memberRequestsToUpdate)
                {
                    mr.Status = RequestStatus.Pending;
                    _unitOfWork.Repository<MembershipRequest>().Update(mr);
                }
                await _unitOfWork.Save();

                return new ResponseHandler().Created<object>("Payment Request Created Successfully");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in CreatePaymentRequestAsync | BatchId: {BatchId}, EntityId: {EntityId}, RequestsCount: {Count}", dto.BatchId, dto.EntityId);

                return new ResponseHandler().BadRequest<object>("Something Went Wrong :)");
            }
        }

        public async Task<Response<object>> CreatePindingPaymentRequest(PindingRequestsDto dto)
        {
            try
            {
                bool isexists = await _unitOfWork.Repository<NewPaymentRequest>().IsExistsAsync(pr => pr.BatchId == dto.BatchId && dto.MemberRequestsIds.Contains(pr.MembershipRequestId));

                if (isexists)
                {
                    return new ResponseHandler().BadRequest<object>("Payment request already exists for the selected membership requests");
                }

                var batchSpecifications = new GeneralSpecifications<Batch>(b => b.Id == dto.BatchId)
                {
                    Includes = new List<Func<IQueryable<Batch>, IQueryable<Batch>>>
                        {
                         IQ=> IQ.Include(b => b.MembershipRequests.Where(mr=>dto.MemberRequestsIds.Contains(mr.Id)))


                        }

                };

                var batch = await _unitOfWork.Repository<Batch>().GetWithSpecifications(batchSpecifications);

                batch.NewPaymentRequests = batch.MembershipRequests.Select(mr => new NewPaymentRequest
                {
                    MembershipRequestId = mr.Id,
                    PaymentStatus = PaymentStatus.Pending,
                    BatchId = batch.Id,
                    EntityId = dto.EntityId,
                    EntityName = batch.TenantName,
                    SuperEntityId = batch.SuperTenentId,
                    SuperEntityName = batch.SuperEntityName,
                    OperationType = dto.OperationType

                }).ToList();

                batch.Status = BatchStatus.Submited;
                _unitOfWork.Repository<Batch>().Update(batch);

                var memberRequestsToUpdate = batch.MembershipRequests
                    .Where(mr => dto.MemberRequestsIds.Contains(mr.Id))
                    .ToList();

                foreach (var mr in memberRequestsToUpdate)
                {
                    mr.Status = RequestStatus.Pending;
                    _unitOfWork.Repository<MembershipRequest>().Update(mr);

                }


                await _unitOfWork.Save();

                return new ResponseHandler().Created<object>("Skiped and Payment Request Created for federation Successfully");

            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<object>("Something Went Wrong :)");
            }

        }

        public async Task<Response<PaginationToReturnDto<GetAllFedrationPaymentRequestToReturnDto>>> GetAllFedrationPaymentRequestsAsync(GetFedrationPaymentRequestDto dto)
        {

            try
            {
                ValidateOnPagination(dto.PageSize, dto.PageSize);

                bool isparsed = int.TryParse(_httpContextAccessor.HttpContext.User.GetTenantId(), out int federationId);

                if (!isparsed)
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<GetAllFedrationPaymentRequestToReturnDto>>("Invalid Federation ID");

                var entityAdmin = await fetchService.GetTenantAdmin(_httpContextAccessor.HttpContext.User.GetUserId()); // Should be replaced with FetchService

                var areasId = entityAdmin.TenantAreaAdmins.Select(areaAdmin => areaAdmin.AreaId).ToHashSet();

                GeneralSpecifications<PaymentBatch> batchSpecifications;

                Expression<Func<PaymentBatch, bool>> paymentBatchCriteria;

                string lowerSearchTerm = string.Empty;

                if (!string.IsNullOrEmpty(dto.SearchTerm))
                {

                    lowerSearchTerm = dto.SearchTerm.ToLower().Replace(" ", "");
                }

                paymentBatchCriteria = pb => pb.SuperEntityId == federationId
                    && (!dto.OperationType.HasValue || pb.OperationType == dto.OperationType)
                    && (!dto.PaymentStatus.HasValue || pb.PaymentStatus == dto.PaymentStatus)
                    && (!dto.DateOfSubmission.HasValue || DateOnly.FromDateTime(pb.CreationDate.Value) == dto.DateOfSubmission)
                    && (!areasId.Any() || areasId.Contains(pb.Entity.AreaId ?? 0))
                      && (string.IsNullOrEmpty(lowerSearchTerm) || pb.EntityName.ToLower().Replace(" ", "").Contains(lowerSearchTerm) || pb.Entity.Area.Name.ToLower().Replace(" ", "").Contains(lowerSearchTerm) || pb.Id.ToString().Contains(lowerSearchTerm));


                batchSpecifications = new GeneralSpecifications<PaymentBatch>(paymentBatchCriteria, dto.PageNumber, dto.PageSize)
                {
                    Includes = new List<Func<IQueryable<PaymentBatch>, IQueryable<PaymentBatch>>>
                        {
                          IQ=> IQ.Include(pb => pb.Batch)
                         .Include(pb=>pb.NewPaymentRequests)
                         .Include(pb=>pb.Entity).ThenInclude(e=>e.Area)
                        }

                };

                batchSpecifications.SetOrderByDescending(pb => pb.CreationDate);


                var fedrationPaymentRequests = await _unitOfWork.Repository<PaymentBatch>().GetAllWithSpecifications(batchSpecifications);

                var totalCount = await _unitOfWork.Repository<PaymentBatch>().GetCountByCriteriaAsync(paymentBatchCriteria);

                var mappedFedrationRequests = _mapper.Map<IReadOnlyList<GetAllFedrationPaymentRequestToReturnDto>>(fedrationPaymentRequests);

                var totalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize);

                var paginatedResult = new PaginationToReturnDto<GetAllFedrationPaymentRequestToReturnDto>(totalCount, totalPages, dto.PageSize, dto.PageNumber, mappedFedrationRequests);

                return new ResponseHandler().Success(paginatedResult);
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<PaginationToReturnDto<GetAllFedrationPaymentRequestToReturnDto>>("Something Went Wrong :)");
            }
        }

        public async Task<Response<object>> GetAllEntityPaymentRequestsAsync(GetPaginationDto dto)
        {

            try
            {
                ValidateOnPagination(dto.PageSize, dto.PageSize);

                bool isparsed = int.TryParse(_httpContextAccessor.HttpContext.User.GetTenantId(), out int entityId);

                if (!isparsed)
                    return new ResponseHandler().BadRequest<object>("Invalid Federation ID");


                string lowerSearchTerm = string.Empty;

                if (!string.IsNullOrEmpty(dto.SearchTerm))
                {

                    lowerSearchTerm = dto.SearchTerm.ToLower().Replace(" ", "");
                }

                Expression<Func<PaymentBatch, bool>> baseCriteria = pb =>
            pb.EntityId == entityId &&
            (!dto.OperationType.HasValue || pb.OperationType == dto.OperationType) &&
            (!dto.PaymentStatus.HasValue || pb.PaymentStatus == dto.PaymentStatus) &&
            (!dto.DateOfSubmission.HasValue ||
                (pb.CreationDate.HasValue && DateOnly.FromDateTime(pb.CreationDate.Value) == dto.DateOfSubmission)) &&
            (string.IsNullOrEmpty(lowerSearchTerm) || pb.Id.ToString().Contains(lowerSearchTerm));




                var batchSpecifications = new GeneralSpecifications<PaymentBatch>(baseCriteria, dto.PageNumber, dto.PageSize)
                {
                    Includes = new List<Func<IQueryable<PaymentBatch>, IQueryable<PaymentBatch>>>
                        {
                         IQ=> IQ.Include(pb => pb.Batch)
                         .Include(pb=>pb.NewPaymentRequests)

                        }

                };

                batchSpecifications.SetOrderByDescending(pb => pb.CreationDate);

                var entityPaymentRequests = await _unitOfWork.Repository<PaymentBatch>()
           .GetAllWithSpecifications(batchSpecifications);


                Expression<Func<PaymentBatch, bool>> withoutPaymentsCriteria = pb =>
                    pb.EntityId == entityId &&
                    pb.PaymentStatus != PaymentStatus.Confirmed &&
                    (!dto.OperationType.HasValue || pb.OperationType == dto.OperationType) &&
                    (!dto.DateOfSubmission.HasValue ||
                        (pb.CreationDate.HasValue && DateOnly.FromDateTime(pb.CreationDate.Value) == dto.DateOfSubmission)) &&
                    (string.IsNullOrEmpty(lowerSearchTerm) || pb.Id.ToString().Contains(lowerSearchTerm));

                int requestsWithoutPayments = await _unitOfWork.Repository<PaymentBatch>()
                    .GetCountByCriteriaAsync(withoutPaymentsCriteria);


                Expression<Func<PaymentBatch, bool>> withPaymentsCriteria = pb =>
                    pb.EntityId == entityId &&
                    pb.PaymentStatus == PaymentStatus.Confirmed &&
                    (!dto.OperationType.HasValue || pb.OperationType == dto.OperationType) &&
                    (!dto.DateOfSubmission.HasValue ||
                        (pb.CreationDate.HasValue && DateOnly.FromDateTime(pb.CreationDate.Value) == dto.DateOfSubmission)) &&
                    (string.IsNullOrEmpty(lowerSearchTerm) || pb.Id.ToString().Contains(lowerSearchTerm));

                int requestsWithPayments = await _unitOfWork.Repository<PaymentBatch>()
                    .GetCountByCriteriaAsync(withPaymentsCriteria);


                int totalCount = await _unitOfWork.Repository<PaymentBatch>()
                    .GetCountByCriteriaAsync(baseCriteria);

                var mappedEntityRequests = _mapper.Map<IReadOnlyList<GetAllEntityPaymentRequestsToReturnDto>>(entityPaymentRequests);

                var totalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize);

                var paginatedResult = new PaginationToReturnDto<GetAllEntityPaymentRequestsToReturnDto>(totalCount, totalPages, dto.PageSize, dto.PageNumber, mappedEntityRequests);


                return new ResponseHandler().Success((object)new
                {
                    Data = mappedEntityRequests,
                    RequestsWithoutPayments = requestsWithoutPayments,
                    RequestsWithPayments = requestsWithPayments,
                    TotalPages = totalPages,
                    PageSize = dto.PageSize,
                    PageNumber = dto.PageNumber,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<object>("Something Went Wrong :)");
            }
        }

        public async Task<Response<GetFedrationPaymentRequestToReturnDto>> GetFedrationPaymentRequestByIdAsync(int paymentRequestId)
        {

            try
            {
                bool isParsed = int.TryParse(_httpContextAccessor.HttpContext.User.GetTenantId(), out int tenantId);



                if (!isParsed)

                    return new ResponseHandler().BadRequest<GetFedrationPaymentRequestToReturnDto>("Invalid tenant ID");

                var batchSpecifications = new GeneralSpecifications<PaymentBatch>(pb => pb.Id == paymentRequestId)
                {
                    Includes = new List<Func<IQueryable<PaymentBatch>, IQueryable<PaymentBatch>>>
                    {
                        IQ=> IQ.Include(pb => pb.NewPaymentRequests)
                        //.Include(pb=>pb.NewPaymentRequests)
                        //.ThenInclude(pr=>pr.MembershipRequest)

                    }

                };




                var fedrationPaymentRequest = await _unitOfWork.Repository<PaymentBatch>().GetWithSpecifications(batchSpecifications);




                if (fedrationPaymentRequest == null)
                {
                    return new ResponseHandler().NotFound<GetFedrationPaymentRequestToReturnDto>("Payment request not found");
                }

                if (fedrationPaymentRequest.EntityId != tenantId && fedrationPaymentRequest.SuperEntityId != tenantId)
                {
                    return new ResponseHandler().BadRequest<GetFedrationPaymentRequestToReturnDto>("Invalid tenant ID");
                }

                var mappedFedrationRequests = _mapper.Map<GetFedrationPaymentRequestToReturnDto>(fedrationPaymentRequest);

                var paymentSummaryDto = await PaymentSummary(fedrationPaymentRequest.NewPaymentRequests.Select(npr => npr.Id).ToList());

                if (!paymentSummaryDto.Succeeded)
                {
                    return new ResponseHandler().BadRequest<GetFedrationPaymentRequestToReturnDto>(paymentSummaryDto.Message);
                }
                mappedFedrationRequests.EntityPaymentSummaryDto = paymentSummaryDto.Data;

                mappedFedrationRequests.BranchAdminConfirmationUrl = await DocumentSetting.GetPreSignedUrlAsync(fedrationPaymentRequest.BranchAdminConfirmationUrl);

                mappedFedrationRequests.ConfirmationUrl = await DocumentSetting.GetPreSignedUrlAsync(fedrationPaymentRequest.ConfirmationUrl);

                mappedFedrationRequests.InvoiceUrl = await DocumentSetting.GetPreSignedUrlsAsync(fedrationPaymentRequest.InvoiceUrl);

                mappedFedrationRequests.RejectionUrl = await DocumentSetting.GetPreSignedUrlAsync(fedrationPaymentRequest.RejectionUrl);

                mappedFedrationRequests.BranchAdminRejectionUrl = await DocumentSetting.GetPreSignedUrlAsync(fedrationPaymentRequest.BranchAdminRejectionUrl);

                return new ResponseHandler().Success(mappedFedrationRequests);
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<GetFedrationPaymentRequestToReturnDto>("Something Went Wrong :)");
            }
        }

        public async Task<Response<string>> ConfirmPaymentRequestAsync(ConfirmPaymentDto dto)
        {
            try
            {
                var isValid = ValidateOnPaymentConfirmation(dto);

                if (!string.IsNullOrEmpty(isValid))
                {
                    return new ResponseHandler().BadRequest<string>("Invalid Payment Confirmation Data");
                }

                var claims = _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet();

                var batchSpecifications = new GeneralSpecifications<PaymentBatch>(pb => pb.Id == dto.PaymentRequestId)
                {
                    Includes = new List<Func<IQueryable<PaymentBatch>, IQueryable<PaymentBatch>>>
                 {
                  IQ=> IQ
                  .Include(pb=>pb.NewPaymentRequests)
                  .ThenInclude(pb=>pb.MembershipRequest)
                  .ThenInclude(mr=>mr.FormUserResponse)
                 },
                    AsSplitQuery = true

                };


                var fedrationPaymentRequest = await _unitOfWork.Repository<PaymentBatch>().GetWithSpecifications(batchSpecifications);

                if (fedrationPaymentRequest == null)
                {
                    return new ResponseHandler().NotFound<string>("Payment request not found");
                }

                bool isParsed = int.TryParse(_httpContextAccessor?.HttpContext?.User.GetTenantId(), out int federationId);

                var userId = _httpContextAccessor?.HttpContext?.User.GetUserId();



                if (!isParsed)

                    return new ResponseHandler().BadRequest<string>("Invalid Federation ID");

                if (fedrationPaymentRequest.SuperEntityId.HasValue && fedrationPaymentRequest.SuperEntityId != federationId)

                    return new ResponseHandler().BadRequest<string>("Invalid Federation ID");


                var userData = new UserDataDto();




                if (claims != null && claims.Contains("BranchAdmin"))
                {
                    if (fedrationPaymentRequest.BranchAdminStatus != BranchAdminStatus.Pending)
                    {
                        return new ResponseHandler().BadRequest<string>("Branch Admin Status is not in pending confirmation status");
                    }
                    //var isBranchAdmin = await _unitOfWork.Repository<Tenant>().IsExistsAsync(t => t.Area.TenantAdminAreas.Any(ta => ta.TenantAdmin.UserId == userId) && t.Id == fedrationPaymentRequest.EntityId);
                    var isBranchAdmin = await IsBranchAdminAsync(userId, fedrationPaymentRequest.EntityId);

                    if (!isBranchAdmin)
                    {
                        return new ResponseHandler().BadRequest<string>("You are not authorized to confirm this payment request as a Branch Admin.");
                    }

                    if (dto.ConfirmationDocument != null)
                    {

                        fedrationPaymentRequest.BranchAdminConfirmationUrl = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", dto.ConfirmationDocument);
                    }

                    fedrationPaymentRequest.BranchAdminConfirmationNotes = dto.ConfirmationNotes;

                    fedrationPaymentRequest.BranchAdminStatus = BranchAdminStatus.Approved;

                    fedrationPaymentRequest.NewPaymentRequests.ToList().ForEach(pr =>
                    {
                        pr.BranchAdminStatus = BranchAdminStatus.Approved;
                    });
                }
                else if (claims != null && claims.Contains("TenantAdmin"))
                {
                    if (fedrationPaymentRequest.PaymentStatus != PaymentStatus.PendingConfirmation)
                    {
                        return new ResponseHandler().BadRequest<string>("Payment request is not in pending confirmation status");
                    }

                    bool isTenantAdmin = await IsTenantAdminAsync(userId, fedrationPaymentRequest.SuperEntityId.Value);

                    if (!isTenantAdmin)
                    {
                        return new ResponseHandler().BadRequest<string>("You are not authorized to confirm this payment request as a Fedration Admin.");
                    }

                    if (dto.ConfirmationDocument != null)
                    {

                        fedrationPaymentRequest.ConfirmationUrl = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", dto.ConfirmationDocument);
                    }

                    fedrationPaymentRequest.ConfirmationNotes = dto.ConfirmationNotes;

                    fedrationPaymentRequest.PaymentStatus = PaymentStatus.Confirmed;

                    fedrationPaymentRequest.NewPaymentRequests.ToList().ForEach(pr =>
                    {
                        pr.PaymentStatus = PaymentStatus.Confirmed;
                    });

                    //bool isAnyMembershipRequestsNotResolved = fedrationPaymentRequest.NewPaymentRequests.Any(npr => npr.MembershipRequest.Status != RequestStatus.RequestResolved);


                    if (fedrationPaymentRequest.OperationType != OperationType.NewMembership)
                    {

                        for (int i = 0; i < fedrationPaymentRequest.NewPaymentRequests.Count; i++)
                        {
                            var membershipRequest = fedrationPaymentRequest.NewPaymentRequests.ElementAtOrDefault(i)?.MembershipRequest;

                            if (membershipRequest != null && membershipRequest.Status == RequestStatus.RequestResolved)
                            {
                                var result = await ExecuteMembershipLogic.ExecuteTransferAndLoanMembershipLogic(_unitOfWork, _mapper, membershipRequest);

                                if (!result.Succeeded)
                                {
                                    return new ResponseHandler().BadRequest<string>(result.Message);
                                }

                                userData.Users.Add(new UserInfoDto { UserId = membershipRequest.UserId, BeltName = membershipRequest.BeltName, UserName = string.Concat(membershipRequest.FirstName, " ", membershipRequest.MiddleName, " ", membershipRequest.LastName) });

                            }

                        }
                    }
                    else
                    {
                        for (int i = 0; i < fedrationPaymentRequest.NewPaymentRequests.Count; i++)
                        {
                            var membershipRequest = fedrationPaymentRequest.NewPaymentRequests.ElementAtOrDefault(i)?.MembershipRequest;

                            if (membershipRequest != null && membershipRequest.Status == RequestStatus.RequestResolved)
                            {
                                var result = await ExecuteMembershipLogic.CheckOutMembership(_formService, _unitOfWork,
                                membershipRequest,
                                membershipRequest.Id
                                );

                                if (!result.Succeeded)
                                {
                                    return new ResponseHandler().BadRequest<string>(result.Message);
                                }

                                userData.Users.Add(new UserInfoDto { UserId = membershipRequest.UserId, BeltName = membershipRequest.BeltName, UserName = string.Concat(membershipRequest.FirstName, " ", membershipRequest.MiddleName, " ", membershipRequest.LastName) });

                            }

                        }
                    }

                }
                else
                    return new ResponseHandler().BadRequest<string>("Invalid Role!");




                await _unitOfWork.Save();
                if (userData.Users.Any())
                {
                    SetUserData(userData, fedrationPaymentRequest);
                    _backgroundJobClient.Enqueue<PublishService>(x => x.PublishUserData(userData));
                }

                return new ResponseHandler().Success("Payment confirmed successfully!");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>("Something Went Wrong :)");
            }
        }


        public async Task<Response<int>> GetBatchFeesByIdAsync(int batchId)
        {
            try
            {
                var batchSpecifications = new GeneralSpecifications<Batch>(b => b.Id == batchId);

                var batch = await _unitOfWork.Repository<Batch>().GetWithSpecifications(batchSpecifications);
                if (batch == null)
                {
                    return new ResponseHandler().NotFound<int>("Batch not found");
                }
                int batchFees = batch.PackagePrice.HasValue ? batch.PackagePrice.Value : 0;
                return new ResponseHandler().Success(batchFees);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<int>("Something Went Wrong :)");
            }
        }


        private string ValidateOnCreatePaymentRequest(AddPaymentRequestDto dto)
        {
            if (dto.AmountPaid <= 0)
            {
                return "Amount paid must be greater than zero";
            }
            if (dto.Invoice != null)
            {
                for (int i = 0; i < dto.Invoice.Count; i++)
                {
                    var isValid = DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(dto.Invoice[i]);

                    if (!isValid)
                    {
                        return "Inalid file format or exceeded max file size";
                    }
                }



            }

            return string.Empty;
        }

        private static void ValidateOnPagination(int pageSize, int pageNumber)
        {
            if (pageSize > 15 || pageSize < 1)
                pageSize = 15;

            if (pageNumber < 1)
                pageNumber = 1;
        }

        public async Task<Response<PaginationToReturnDto<GetAllFedrationEntitiesPaymentRequestToReturnDto>>> GetAllFedrationEntitiesPaymentRequestsAsync(GetFedrationEntityPaymentRequestDto dto)
        {
            try
            {
                ValidateOnPagination(dto.PageSize, dto.PageSize);

                bool isParsed = int.TryParse(_httpContextAccessor.HttpContext.User.GetTenantId(), out int federationId);
                if (!isParsed)
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<GetAllFedrationEntitiesPaymentRequestToReturnDto>>("Invalid Federation ID");

                var tenantAdmins = await fetchService.GetTenantAdmin(_httpContextAccessor.HttpContext.User.GetUserId());

                GeneralSpecifications<NewPaymentRequest> specification;

                Expression<Func<NewPaymentRequest, bool>> paymentCriteria;

                string lowerSearchTerm = string.Empty;

                if (!string.IsNullOrEmpty(dto.SearchTerm))
                {

                    lowerSearchTerm = dto.SearchTerm.ToLower().Replace(" ", "");
                }

                var areasId = tenantAdmins.TenantAreaAdmins.Select(area => area.AreaId).ToList();




                paymentCriteria = r => r.SuperEntityId == federationId
                    && (!areasId.Any() || areasId.Contains(r.Entity.AreaId ?? 0))
                    && (!dto.BranchId.HasValue || r.Entity.AreaId == dto.BranchId.Value) &&
                   (string.IsNullOrEmpty(lowerSearchTerm) || r.Entity.Name.ToLower().Replace(" ", "").Contains(lowerSearchTerm));
                specification = new GeneralSpecifications<NewPaymentRequest>(paymentCriteria)
                {
                    Includes =
                        [
                            q => q.Include(r => r.Entity)
                            .ThenInclude(r => r.Area)
                        ]
                };



                var allRequests = await _unitOfWork.Repository<NewPaymentRequest>()
                    .GetAllWithSpecifications(specification);

                var groupedResult = allRequests
                    .GroupBy(r => r.EntityName)
                    .Select(g => new GetAllFedrationEntitiesPaymentRequestToReturnDto
                    {
                        Id = g.AsEnumerable().FirstOrDefault()?.EntityId ?? 0,
                        EntityName = g.Key,
                        BranchName = g.AsEnumerable().FirstOrDefault()?.Entity?.Area?.Name ?? "Unknown Branch",
                        NumberOfRequests = g.Count(),
                        RequestsWithPayment = g.Count(x => x.PaymentStatus != PaymentStatus.Pending),
                        RequestsWithoutPayment = g.Count(x => x.PaymentStatus == PaymentStatus.Pending)
                    })
                    .ToList();


                // Step 3: Apply pagination manually on grouped result
                var totalCount = groupedResult.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize);

                var paginatedItems = groupedResult
                    .Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToList();

                var paginatedResult = new PaginationToReturnDto<GetAllFedrationEntitiesPaymentRequestToReturnDto>(totalCount, totalPages, dto.PageSize, dto.PageNumber, paginatedItems);


                return new ResponseHandler().Success(paginatedResult);
            }
            catch (Exception)
            {
                return new ResponseHandler().BadRequest<PaginationToReturnDto<GetAllFedrationEntitiesPaymentRequestToReturnDto>>("Something Went Wrong :)");
            }
        }

        public async Task<Response<PaginationToReturnDto<GetAllEntityPaymentRequestToReturnDto>>> GetAllEntityPaymentRequest(GetEntitiyPaymentRequestDto dto)
        {
            try
            {
                ValidateOnPagination(dto.PageSize, dto.PageSize);


                GeneralSpecifications<NewPaymentRequest> batchSpecifications;

                Expression<Func<NewPaymentRequest, bool>> paymentCriteria;

                string lowerSearchTerm = string.Empty;

                if (!string.IsNullOrEmpty(dto.SearchTerm))
                {

                    lowerSearchTerm = dto.SearchTerm.ToLower().Replace(" ", "");
                }
                paymentCriteria = pb =>
                 pb.EntityId == dto.EntityId &&
                 (!dto.PaymentStatus.HasValue || pb.PaymentStatus == dto.PaymentStatus.Value) &&
                 (
                     string.IsNullOrEmpty(lowerSearchTerm) ||
                     (pb.MembershipRequest.FileNumber != null && pb.MembershipRequest.FileNumber.ToLower().Replace(" ", "").Contains(lowerSearchTerm)) ||
                     (pb.Batch != null && pb.Batch.BatchNumber.ToLower().Replace(" ", "").Contains(lowerSearchTerm)) ||
                     (pb.MembershipRequest.FirstName != null && pb.MembershipRequest.FirstName.ToLower().Replace(" ", "").Contains(lowerSearchTerm)) ||
                     (pb.MembershipRequest.LastName != null && pb.MembershipRequest.LastName.ToLower().Replace(" ", "").Contains(lowerSearchTerm)) ||
                     (pb.Entity != null && pb.Entity.Name.ToLower().Replace(" ", "").Contains(lowerSearchTerm))
                 );

                batchSpecifications = new GeneralSpecifications<NewPaymentRequest>(paymentCriteria, dto.PageNumber, dto.PageSize)
                {
                    Includes = new List<Func<IQueryable<NewPaymentRequest>, IQueryable<NewPaymentRequest>>>
                    {
                        q => q.Include(x => x.PaymentBatch)
                              .ThenInclude(pb => pb.Batch)
                                .ThenInclude(pb => pb.RequestType),
                        q => q.Include(x => x.Batch),
                        q => q.Include(x => x.MembershipRequest)
                              .ThenInclude(mr => mr.FormUserResponse)
                                  .ThenInclude(fur => fur.FieldsResponse)
                                      .ThenInclude(fr => fr.Field),
                        q => q.Include(x => x.PaymentBatch)
                        .ThenInclude(pb => pb.Entity)
                        .ThenInclude(pb => pb.Area)

                    }


                };


                var paymentRequests = await _unitOfWork.Repository<NewPaymentRequest>().GetAllWithSpecifications(batchSpecifications);

                var totalCount = await _unitOfWork.Repository<NewPaymentRequest>()
                    .GetCountByCriteriaAsync(r => r.EntityId == dto.EntityId);

                var resultDtos = _mapper.Map<List<GetAllEntityPaymentRequestToReturnDto>>(paymentRequests);

                var totalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize);

                var paginatedResult = new PaginationToReturnDto<GetAllEntityPaymentRequestToReturnDto>(
                    totalCount,
                    totalPages,
                    dto.PageSize,
                    dto.PageNumber,
                    resultDtos
                );

                return new ResponseHandler().Success(paginatedResult);
            }
            catch (Exception ex)
            {
                // Ideally log ex
                return new ResponseHandler().BadRequest<PaginationToReturnDto<GetAllEntityPaymentRequestToReturnDto>>("Something went wrong :)");
            }
        }

        public async Task<Response<string>> EntityPayNowAsync(EntityPayNowDto dto)
        {
            try
            {
                if (dto.AmountPaid <= 0)
                {
                    return new ResponseHandler().BadRequest<string>("Amount paid must be greater than zero");
                }

                List<string> invoiceUrl = new List<string>();

                if (dto.Invoice != null)
                {
                    for (int i = 0; i < dto.Invoice.Count; i++)
                    {
                        bool isValid = DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(dto.Invoice[0]);


                        if (!isValid)
                            return new ResponseHandler().BadRequest<string>("Invalid file format or exceeded max file size");


                        invoiceUrl.Add(await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", dto.Invoice[i]));

                    }

                }

                var paymentSpecifications = new GeneralSpecifications<PaymentBatch>(pb => pb.Id == dto.PaymentRequestId)
                {
                    Includes = new List<Func<IQueryable<PaymentBatch>, IQueryable<PaymentBatch>>>
                        {
                         IQ=> IQ.Include(pb => pb.NewPaymentRequests)

                        }

                };



                var paymentBatch = await _unitOfWork.Repository<PaymentBatch>().GetWithSpecifications(paymentSpecifications);

                paymentBatch.InvoiceUrl = invoiceUrl;

                paymentBatch.AmountPaid = dto.AmountPaid;

                if (paymentBatch == null)
                {
                    return new ResponseHandler().NotFound<string>("Payment request not found");
                }
                if (paymentBatch.PaymentStatus != PaymentStatus.PendingPayment)
                {
                    return new ResponseHandler().BadRequest<string>("Payment request is not in pending payment status");
                }

                var paymentRequests = paymentBatch.NewPaymentRequests
                     .Where(pr => pr.PaymentBatchId == paymentBatch.Id && pr.PaymentStatus == PaymentStatus.PendingPayment)
                     .ToList();
                foreach (var pr in paymentRequests)
                {
                    pr.PaymentStatus = PaymentStatus.PendingConfirmation;

                }

                paymentBatch.PaymentStatus = PaymentStatus.PendingConfirmation;

                await _unitOfWork.Save();
                return new ResponseHandler().Success("Payment initiated successfully!");
            }
            catch (Exception ex)
            {
                // Log ex here
                return new ResponseHandler().BadRequest<string>("Failed to initiate payment.");
            }
        }

        public async Task<Response<EntityPaymentRequestStatusSummaryDto>> GetPaymentRequestStatusSummaryAsync(int entityId)
        {
            try
            {
                var spasification = new GeneralSpecifications<NewPaymentRequest>(r => r.EntityId == entityId)
                {
                    Includes = new List<Func<IQueryable<NewPaymentRequest>, IQueryable<NewPaymentRequest>>>
                        {
                         IQ=> IQ.Include(pb => pb.Entity)

                        }

                };
                var requests = await _unitOfWork.Repository<NewPaymentRequest>()
                    .GetAllWithSpecifications(spasification);

                var summary = new EntityPaymentRequestStatusSummaryDto
                {
                    EntityName = requests.FirstOrDefault()?.EntityName ?? "Unknown Entity",
                    Image = requests.FirstOrDefault()?.Entity?.Image ?? string.Empty,
                    TotalRequests = requests.Count,
                    UnpaidRequests = requests.Count(r => r.PaymentStatus == PaymentStatus.Pending),
                    PendingPayment = requests.Count(r => r.PaymentStatus == PaymentStatus.PendingPayment),
                    PendingConfirmation = requests.Count(r => r.PaymentStatus == PaymentStatus.PendingConfirmation),
                    ConfirmedRequests = requests.Count(r => r.PaymentStatus == PaymentStatus.Confirmed)
                };

                return new ResponseHandler().Success(summary);
            }
            catch (Exception ex)
            {
                // Log ex here
                return new ResponseHandler().BadRequest<EntityPaymentRequestStatusSummaryDto>("Failed to get payment status summary.");
            }
        }

        public async Task<Response<FederationPaymentSummaryDto>> GetFederationPaymentSummaryAsync()
        {
            try
            {
                bool isParsed = int.TryParse(_httpContextAccessor.HttpContext.User.GetTenantId(), out int federationId);
                if (!isParsed)
                    return new ResponseHandler().BadRequest<FederationPaymentSummaryDto>("Invalid Federation ID");


                var userId = _httpContextAccessor.HttpContext.User.GetUserId();
                var admin = await fetchService.GetTenantAdmin(userId);
                var areaIds = admin.TenantAreaAdmins.Select(area => area.AreaId).ToList();

                GeneralSpecifications<NewPaymentRequest> specifications;



                specifications = new GeneralSpecifications<NewPaymentRequest>(r => r.SuperEntityId == federationId && (!areaIds.Any() || areaIds.Contains(r.Entity.AreaId ?? 0)))
                {
                    Includes =
                       [
                           Iq => Iq.Include(p => p.Entity)
                       ]
                };

                var requests = await _unitOfWork.Repository<NewPaymentRequest>().GetAllWithSpecifications(specifications);

                var summary = new FederationPaymentSummaryDto
                {
                    NumberOfEntities = requests.Select(r => r.EntityId).Distinct().Count(),
                    NumberOfRequests = requests.Count,
                    RequestsWithoutPayment = requests.Count(r =>
                        r.PaymentStatus == PaymentStatus.Pending ||
                        r.PaymentStatus == PaymentStatus.PendingPayment ||
                        r.PaymentStatus == PaymentStatus.PendingConfirmation),
                    RequestsWithPayment = requests.Count(r => r.PaymentStatus == PaymentStatus.Confirmed)
                };

                return new ResponseHandler().Success(summary);
            }
            catch (Exception ex)
            {
                // Log ex here
                return new ResponseHandler().BadRequest<FederationPaymentSummaryDto>("Failed to get entity payment summaries.");
            }
        }

        public async Task<Response<string>> PaymentGenerationAsync(List<int> requestIds)
        {
            try
            {
                if (requestIds == null || !requestIds.Any())
                    return new ResponseHandler().BadRequest<string>("No request IDs provided.");

                // Fetch all NewPaymentRequests matching the provided MembershipRequest IDs
                var paymentRequests = await _unitOfWork.Repository<NewPaymentRequest>()
                    .GetAllAsync(pr => requestIds.Contains(pr.MembershipRequestId));

                if (!paymentRequests.Any())
                    return new ResponseHandler().NotFound<string>("No matching payment requests found.");

                foreach (var pr in paymentRequests)
                {
                    pr.PaymentStatus = PaymentStatus.PendingPayment;

                    //  _unitOfWork.Repository<NewPaymentRequest>().UpdateField(pr, r => r.PaymentStatus);
                }

                await _unitOfWork.Save();

                return new ResponseHandler().Success("Payment statuses updated to 'PendingPayment' successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>(ex.InnerException?.Message ?? ex.Message);
            }
        }


        public async Task<Response<string>> GenereatePaymentRequests(List<int> requestIds)
        {
            try
            {
                if (requestIds == null || !requestIds.Any())
                    return new ResponseHandler().BadRequest<string>("No request IDs provided.");

                var inputRequestIdsSet = requestIds.ToHashSet();

                var specification = new GeneralSpecifications<NewPaymentRequest>(
                    r => inputRequestIdsSet.Contains(r.Id));


                var paymentRequests = await _unitOfWork.Repository<NewPaymentRequest>()
                                                       .GetAllWithSpecifications(specification);

                if (paymentRequests.Any(pr => pr.PaymentBatchId != null))
                {
                    return new ResponseHandler().BadRequest<string>("Some payment requests already have a payment batch.");
                }
                var paymentSummary = await PaymentSummary(requestIds);

                if (!paymentSummary.Succeeded)
                    return new ResponseHandler().BadRequest<string>(paymentSummary.Message);

                await _unitOfWork.Repository<PaymentBatch>().Add(new PaymentBatch
                {
                    BatchId = paymentRequests[0].BatchId,
                    EntityId = paymentRequests[0].EntityId,
                    EntityName = paymentRequests[0].EntityName,
                    SuperEntityName = paymentRequests[0].SuperEntityName,
                    SuperEntityId = paymentRequests[0].SuperEntityId,
                    PaymentStatus = PaymentStatus.PendingPayment,
                    OperationType = paymentRequests[0].OperationType,
                    NewPaymentRequests = paymentRequests,
                    TotalAmountDue = paymentSummary.Data.TotalAmountDue.HasValue ? paymentSummary.Data.TotalAmountDue.Value : 0,
                });


                foreach (var pr in paymentRequests)
                {

                    pr.PaymentStatus = PaymentStatus.PendingPayment;


                }

                await _unitOfWork.Save();

                return new ResponseHandler().Success("Payment statuses updated to 'PendingPayment' successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<object>>> GetEntityBankAcounts()
        {
            try
            {
                bool isParsed = int.TryParse(_httpContextAccessor.HttpContext.User.GetTenantId(), out int entityId);
                if (!isParsed)
                    return new ResponseHandler().BadRequest<List<object>>("Invalid Entity ID");

                var entitySpecification = new GeneralSpecifications<Tenant>(t => t.Id == entityId)
                {
                    Includes = new List<Func<IQueryable<Tenant>, IQueryable<Tenant>>>
                        {
                         IQ=> IQ.Include(t => t.Area).ThenInclude(Area => Area.BankAccounts)
                        }
                };

                var entity = await _unitOfWork.Repository<Tenant>().GetWithSpecifications(entitySpecification);

                if (entity == null)
                {
                    return new ResponseHandler().NotFound<List<object>>("Invalid Entity ID");
                }
                var banksToReturnDto = entity.Area?.BankAccounts
                    .Select(b => (object)new
                    {
                        Id = b.Id,
                        BankName = b.BankName,
                        AccountNumber = b.AccountNumber,

                    }).ToList();
                return new ResponseHandler().Success(banksToReturnDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<object>>("Something Went Wrong :)");
            }
        }

        public async Task<Response<EntityPaymentSummaryDto>> EntityPaymentSummary(List<int> memberRequestIds)
        {
            //Id = 6, Name = "New Membership Request"
            //Id = 19, Name = "Transfer"
            //Id = 20, Name = "Loan"

            const int NewMembershipRequestId = 6;
            const int TransferId = 19;
            const int LoanId = 20;

            var inputRequestIdsSet = memberRequestIds.ToHashSet();

            var memberRequestsSpecifications = new GeneralSpecifications<MembershipRequest>(pb => inputRequestIdsSet.Contains(pb.Id))
            {


            };




            var memberRequests = await _unitOfWork.Repository<MembershipRequest>().GetAllWithSpecifications(memberRequestsSpecifications);

            var entityId = memberRequests?.Select(pr => pr.TenantId).FirstOrDefault();

            var operationType = memberRequests.Select(pr => pr.OperationType).FirstOrDefault();

            int operationTypeId = operationType == OperationType.NewMembership ? NewMembershipRequestId :
                operationType == OperationType.Transfer ? TransferId : LoanId;

            var entitySpecification = new GeneralSpecifications<Tenant>(t => t.Id == entityId)
            {
                Includes = new List<Func<IQueryable<Tenant>, IQueryable<Tenant>>>
                        {
                         IQ=> IQ.Include(t => t.Area)
                         .ThenInclude(Area => Area.AreaPrices)
                         .Include(t => t.Area)
                         .ThenInclude(Area => Area.AreaConfigurations.Where(ac => ac.OperationId == operationTypeId)
                 )
                         //.ThenInclude(ac=>ac.Form)



                        }

            };

            var entity = await _unitOfWork.Repository<Tenant>().GetWithSpecifications(entitySpecification);

            bool? isExists = entity?.Area?.AreaConfigurations.Any(ac => ac.OperationId == operationTypeId);

            if (isExists == null || !isExists.Value)
            {
                return new ResponseHandler().BadRequest<EntityPaymentSummaryDto>("Operation type not found in area configurations.");
            }

            var membersPaymentDto = memberRequests?.Select(mr => new MembersPaymentDto
            {

                MembershipRequestId = mr.Id,
                MemberName = string.Concat(mr.FirstName, " ", mr.LastName),
                BeltName = mr.BeltName,
                BeltPrice = 0,
            }).ToList();

            var belts = membersPaymentDto?.Select(m => m.BeltName).ToHashSet();

            var beltPrices = entity?.Area?.AreaPrices?
   .Where(ap => belts != null && ap.BeltName != null && belts.Contains(ap.BeltName))
   .ToDictionary(
       ap => ap.BeltName!,
       ap => operationType == OperationType.NewMembership ? ap.MembershipPrice :
             operationType == OperationType.Transfer ? ap.TransferPrice :
             ap.LoanPrice
   );
            if (membersPaymentDto != null)

                foreach (var item in membersPaymentDto)
                {
                    var beltName = item.BeltName ?? string.Empty;

                    item.BeltPrice = beltPrices != null && beltPrices.TryGetValue(beltName, out var price)
                        ? price
                        : 0;
                }

            var formFees = entity?.Area?.AreaConfigurations?.FirstOrDefault()?.FormFee ?? 0;
            var totalFormFees = formFees * membersPaymentDto?.Count ?? 0;
            var totalFees = membersPaymentDto?.Sum(m => m.BeltPrice) ?? 0;
            var branchFee = (entity?.Area?.AreaConfigurations?.FirstOrDefault()?.AreaFee ?? 0) * membersPaymentDto?.Count ?? 0;
            var operatingFee = (entity?.Area?.AreaConfigurations?.FirstOrDefault()?.AreaOperatingFees ?? 0) * membersPaymentDto?.Count ?? 0;
            var vat = (entity?.Area?.AreaConfigurations?.FirstOrDefault()?.AreaVAT ?? 0) * membersPaymentDto?.Count ?? 0;

            var paymentSummaryDto = new EntityPaymentSummaryDto
            {
                MembersPaymentDto = membersPaymentDto ?? new List<MembersPaymentDto>(),
                TotalFormFees = formFees * membersPaymentDto?.Count ?? 0,
                FormFees = formFees,
                TotalFees = totalFees,
                BranchFee = branchFee,
                OperatingFees = operatingFee,
                Vat = vat,
                TotalAmountDue = totalFormFees + totalFees + branchFee + operatingFee + vat
            };

            return new ResponseHandler().Success(paymentSummaryDto);
        }

        public async Task<Response<EntityPaymentSummaryDto>> GetFedrationPaymentSummariesByRequestIdsAsync(List<int> paymentRequestIds)
        {
            try
            {
                if (paymentRequestIds == null || !paymentRequestIds.Any())
                    return new ResponseHandler().BadRequest<EntityPaymentSummaryDto>("No payment request IDs provided.");

                var summaryDto = await PaymentSummary(paymentRequestIds);

                if (!summaryDto.Succeeded)
                    return new ResponseHandler().BadRequest<EntityPaymentSummaryDto>(summaryDto.Message);

                return new ResponseHandler().Success(summaryDto.Data);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EntityPaymentSummaryDto>("Something went wrong while processing the summary.");
            }
        }

        private async Task<Response<EntityPaymentSummaryDto>> PaymentSummary(List<int> requestIds)
        {
            //Id = 6, Name = "New Membership Request"
            //Id = 19, Name = "Transfer"
            //Id = 20, Name = "Loan"

            const int NewMembershipRequestId = 6;
            const int TransferId = 19;
            const int LoanId = 20;


            var inputRequestIdsSet = requestIds.ToHashSet();

            var paymentRequestsSpecifications = new GeneralSpecifications<NewPaymentRequest>(pb => inputRequestIdsSet.Contains(pb.Id))
            {
                Includes = new List<Func<IQueryable<NewPaymentRequest>, IQueryable<NewPaymentRequest>>>
                {
                 IQ=> IQ.Include(pb => pb.MembershipRequest)



                }

            };




            var paymentRequests = await _unitOfWork.Repository<NewPaymentRequest>().GetAllWithSpecifications(paymentRequestsSpecifications);

            var operationType = paymentRequests.Select(pr => pr.OperationType).FirstOrDefault();

            int operationTypeId = operationType == OperationType.NewMembership ? NewMembershipRequestId :
                operationType == OperationType.Transfer ? TransferId : LoanId;


            var entityId = paymentRequests?.Select(pr => pr.EntityId).FirstOrDefault();

            var entitySpecification = new GeneralSpecifications<Tenant>(t => t.Id == entityId)
            {
                Includes = new List<Func<IQueryable<Tenant>, IQueryable<Tenant>>>
                {
                 IQ=> IQ.Include(t => t.Area)
                 .ThenInclude(Area => Area.AreaPrices)
                 .Include(t => t.Area)
                 .ThenInclude(Area => Area.AreaConfigurations.Where(ac => ac.OperationId ==operationTypeId)
                 )
                 //.ThenInclude(ac=>ac.Form)



                }

            };

            var entity = await _unitOfWork.Repository<Tenant>().GetWithSpecifications(entitySpecification);

            bool? isExists = entity?.Area?.AreaConfigurations.Any(ac => ac.OperationId == operationTypeId);

            if (isExists == null || !isExists.Value)
            {
                return new ResponseHandler().BadRequest<EntityPaymentSummaryDto>("Operation type not found in area configurations.");
            }

            var membersPaymentDto = paymentRequests?.Select(pr => new MembersPaymentDto
            {
                PaymentRequestId = pr.Id,
                MembershipRequestId = pr.MembershipRequest.Id,
                MemberName = string.Concat(pr.MembershipRequest.FirstName, " ", pr.MembershipRequest.LastName),
                BeltName = pr.MembershipRequest.BeltName,
                BeltPrice = 0,
            }).ToList();

            var belts = membersPaymentDto?.Select(m => m.BeltName).ToHashSet();

            var beltPrices = entity?.Area?.AreaPrices?
    .Where(ap => belts != null && ap.BeltName != null && belts.Contains(ap.BeltName))
    .ToDictionary(
        ap => ap.BeltName!,
        ap => operationType == OperationType.NewMembership ? ap.MembershipPrice :
              operationType == OperationType.Transfer ? ap.TransferPrice :
              ap.LoanPrice
    );
            if (membersPaymentDto != null)

                foreach (var item in membersPaymentDto)
                {
                    var beltName = item.BeltName ?? string.Empty;

                    item.BeltPrice = beltPrices != null && beltPrices.TryGetValue(beltName, out var price)
                        ? price
                        : 0;
                }
            var formFees = entity?.Area?.AreaConfigurations?.FirstOrDefault()?.FormFee ?? 0;
            var totalFormFees = formFees * membersPaymentDto?.Count ?? 0;
            var totalFees = membersPaymentDto?.Sum(m => m.BeltPrice) ?? 0;
            var branchFee = (entity?.Area?.AreaConfigurations?.FirstOrDefault()?.AreaFee ?? 0) * membersPaymentDto?.Count ?? 0;
            var operatingFee = (entity?.Area?.AreaConfigurations?.FirstOrDefault()?.AreaOperatingFees ?? 0) * membersPaymentDto?.Count ?? 0;
            var vat = (entity?.Area?.AreaConfigurations?.FirstOrDefault()?.AreaVAT ?? 0) * membersPaymentDto?.Count ?? 0;

            var paymentSummaryDto = new EntityPaymentSummaryDto
            {
                MembersPaymentDto = membersPaymentDto ?? new List<MembersPaymentDto>(),
                TotalFormFees = formFees * membersPaymentDto?.Count ?? 0,
                FormFees = formFees,
                TotalFees = totalFees,
                BranchFee = branchFee,
                OperatingFees = operatingFee,
                Vat = vat,
                TotalAmountDue = totalFormFees + totalFees + branchFee + operatingFee + vat
            };

            return new ResponseHandler().Success(paymentSummaryDto);

        }


        public async Task<Response<string>> RejectPaymentAsync(RejectPaymentDto dto)
        {
            try
            {

                var isValid = ValidateOnPaymentRejection(dto);

                if (!string.IsNullOrEmpty(isValid))
                {
                    return new ResponseHandler().BadRequest<string>(isValid);
                }

                var claims = _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet();


                var batchSpecifications = new GeneralSpecifications<PaymentBatch>(pb => pb.Id == dto.PaymentRequestId)
                {
                    Includes = new List<Func<IQueryable<PaymentBatch>, IQueryable<PaymentBatch>>>
                {
                 IQ=> IQ.Include(pb => pb.NewPaymentRequests)
                }
                };

                var paymentBatch = await _unitOfWork.Repository<PaymentBatch>().GetWithSpecifications(batchSpecifications);

                if (paymentBatch == null)
                {
                    return new ResponseHandler().NotFound<string>("Payment request not found");
                }

                bool isParsed = int.TryParse(_httpContextAccessor?.HttpContext?.User.GetTenantId(), out int federationId);

                if (!isParsed)

                    return new ResponseHandler().BadRequest<string>("Invalid Federation ID");

                if (paymentBatch.SuperEntityId.HasValue && paymentBatch.SuperEntityId != federationId)

                    return new ResponseHandler().BadRequest<string>("Invalid Federation ID");


                if (paymentBatch.PaymentStatus != PaymentStatus.PendingConfirmation)
                {
                    return new ResponseHandler().BadRequest<string>("Payment request is not in pending confirmation status");
                }






                var userId = _httpContextAccessor?.HttpContext?.User.GetUserId();

                if (claims != null && claims.Contains("BranchAdmin"))
                {
                    bool isBranchAdmin = await IsBranchAdminAsync(userId, paymentBatch.EntityId);

                    if (!isBranchAdmin)
                    {
                        return new ResponseHandler().BadRequest<string>("You are not authorized to confirm this payment request as a Fedration Admin.");
                    }

                    if (dto.RejectionDocument != null)
                    {

                        paymentBatch.BranchAdminRejectionUrl = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", dto.RejectionDocument);
                    }

                    paymentBatch.BranchAdminReason = dto.Reason;

                    paymentBatch.BranchAdminStatus = BranchAdminStatus.Rejected;

                    paymentBatch.NewPaymentRequests.ToList().ForEach(pr =>
                    {
                        pr.BranchAdminStatus = BranchAdminStatus.Rejected;
                    });
                }
                else if (claims != null && claims.Contains("TenantAdmin"))
                {
                    bool isTenantAdmin = await IsTenantAdminAsync(userId, paymentBatch.SuperEntityId.Value);

                    if (!isTenantAdmin)
                    {
                        return new ResponseHandler().BadRequest<string>("You are not authorized to confirm this payment request as a Fedration Admin.");
                    }

                    if (dto.RejectionDocument != null)
                    {

                        paymentBatch.RejectionUrl = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", dto.RejectionDocument);
                    }

                    paymentBatch.Reason = dto.Reason;

                    paymentBatch.PaymentStatus = PaymentStatus.Rejected;



                    paymentBatch.NewPaymentRequests.ToList().ForEach(pr =>
                    {
                        pr.PaymentStatus = PaymentStatus.Rejected;
                    });

                    var newPaymentBatch = new PaymentBatch
                    {
                        BatchId = paymentBatch.BatchId,
                        EntityId = paymentBatch.EntityId,
                        EntityName = paymentBatch.EntityName,
                        SuperEntityId = paymentBatch.SuperEntityId,
                        SuperEntityName = paymentBatch.SuperEntityName,
                        PaymentStatus = PaymentStatus.PendingPayment,
                        InvoiceUrl = paymentBatch.InvoiceUrl,
                        PaymentType = paymentBatch.PaymentType,
                        TotalAmountDue = paymentBatch.TotalAmountDue,
                        AreaConfigurationId = paymentBatch.AreaConfigurationId,
                        OperationType = paymentBatch.OperationType,

                        NewPaymentRequests = paymentBatch.NewPaymentRequests.Select(pr => new NewPaymentRequest
                        {
                            MembershipRequestId = pr.MembershipRequestId,
                            PaymentStatus = PaymentStatus.PendingPayment,
                            BatchId = paymentBatch.BatchId,
                            EntityId = paymentBatch.EntityId,
                            EntityName = paymentBatch.EntityName,
                            SuperEntityId = paymentBatch.SuperEntityId,
                            SuperEntityName = paymentBatch.SuperEntityName,
                            OperationType = paymentBatch.OperationType,
                        }).ToList()

                    };

                    await _unitOfWork.Repository<PaymentBatch>().Add(newPaymentBatch);
                }
                else
                    return new ResponseHandler().BadRequest<string>("Invalid Role!");




                await _unitOfWork.Save();

                return new ResponseHandler().Created("Payment request rejected successfully and new payment request created for pending payments.");
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<string>("Something Went Wrong :)");
            }

        }

        private string ValidateOnPaymentRejection(RejectPaymentDto dto)
        {
            if (dto.RejectionDocument != null)
            {
                var isValid = DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(dto.RejectionDocument);

                if (!isValid)
                {
                    return "Invalid file format or exceeded max file size";
                }
            }

            if (string.IsNullOrEmpty(dto.Reason))
            {
                return "Reason for rejection is required.";
            }

            return string.Empty;
        }

        private string ValidateOnPaymentConfirmation(ConfirmPaymentDto dto)
        {
            if (dto.ConfirmationDocument != null)
            {
                var isValid = DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(dto.ConfirmationDocument);

                if (!isValid)
                {
                    return "Invalid file format or exceeded max file size";
                }
            }

            //if (string.IsNullOrEmpty(dto.ConfirmationNotes))
            //{
            //    return "Notes for confirmation is required.";
            //}

            return string.Empty;
        }

        private async Task<bool> IsBranchAdminAsync(string userId, int entityId)
            => await _unitOfWork.Repository<TenantAdminArea>().IsExistsAsync(TenantAdminArea => TenantAdminArea.TenantAdmin.UserId == userId && TenantAdminArea.Area.Entities.Any(t => t.Id == entityId));


        private async Task<bool> IsTenantAdminAsync(string userId, int entityId)
            => await _unitOfWork.Repository<TenantAdmin>().IsExistsAsync(t => t.UserId == userId && t.TenantId == entityId);

        private void SetUserData(UserDataDto userData, PaymentBatch paymentBatch)
        {
            userData.TenantData.EntityId = paymentBatch.EntityId;
            userData.TenantData.EntityName = paymentBatch?.EntityName!;
            userData.TenantData.SuperEntityId = paymentBatch?.SuperEntityId ?? 0;
            userData.TenantData.SuperEntityName = paymentBatch?.SuperEntityName!;

        }

    }


}
