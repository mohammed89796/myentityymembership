using AutoMapper;
using Common.RabbitMq.Membership;
using Common.RabbitMq.Tenant;
using Common.RabbitMq.Users;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos;
using MyEntityMembershipService.DTOs.Membership.SwimmingMembershipRequestDtos;
using MyEntityMembershipService.DTOs.Membership.UserDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.MembershipRequestRepository;
using MyEntityMembershipService.Repositories.Membership.RequestTypeRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.Services.Membership.FormServ;
using MyEntityMembershipService.Services.Membership.MembershipServ;
using MyEntityMembershipService.Services.Membership.MemberTypesServ;
using MyEntityMembershipService.Services.Membership.RequestPaymentServ;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Security.Claims;

namespace MyEntityMembershipService.Services.Membership.MembershipRequestWebServ
{
    public class MembershipRequestWebService : IMembershipRequestWebService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IMembershipRequestRepository membershipRequestRepository;
        private readonly IRequestTypeRepository requestTypeRepository;
        private readonly IPaymentRequestService paymentRequestService;
        private readonly IMembershipService membershipService;
        private readonly IMemberTypeService memberTypeService;
        private readonly IBackgroundJobClient backgroundJobClient;

        private readonly IFormService formService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IFetchService fetchService;
        private readonly MassTransit.IPublishEndpoint _publishEndpoint;
        private readonly IHttpContextAccessor _httpContextAccessor;



        private readonly IMemoryCache memoryCache;


        public MembershipRequestWebService(IUnitOfWork unitOfWork, IMapper mapper,
                                        IConfiguration configuration, IMembershipRequestRepository membershipRequestRepository,
                                         IRequestTypeRepository requestTypeRepository, IPaymentRequestService paymentRequestService, IFormService formService,
                                         IHttpContextAccessor httpContextAccessor, IMembershipService membershipService, IMemberTypeService memberTypeService, IBackgroundJobClient backgroundJobClient, IFetchService fetchService, IMemoryCache memoryCache, MassTransit.IPublishEndpoint publishEndpoint)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.configuration = configuration;
            this.membershipRequestRepository = membershipRequestRepository;
            this.requestTypeRepository = requestTypeRepository;
            this.paymentRequestService = paymentRequestService;
            this.formService = formService;
            this.httpContextAccessor = httpContextAccessor;
            this.membershipService = membershipService;
            this.memberTypeService = memberTypeService;
            this.backgroundJobClient = backgroundJobClient;
            this.fetchService = fetchService;
            this.memoryCache = memoryCache;
            _publishEndpoint = publishEndpoint;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<Response<List<MembershipRequest>>> GetAllRequest()
        {
            try
            {
                var request = await unitOfWork.Repository<MembershipRequest>().GetAllAsync();

                return new ResponseHandler().Success(request);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MembershipRequest>>(ex.Message);
            }
        }

        public async Task<Response<GetAllRequestsGroupedResponseDto>> GetAllRequestsGrouped(int tenantId)
        {
            try
            {
                var request = await membershipRequestRepository.GetAllRequestsGrouped(tenantId);

                return new ResponseHandler().Success(request);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetAllRequestsGroupedResponseDto>(ex.Message);
            }
        }

        public async Task<Response<PaginationToReturnDto<MembershipRequestDto>>> GetRequestsByTenantId(NewMembershipRequestsDto? newMembershipRequestsDto, int tenantId, int pageSize = 5, int pageNumber = 1)
        {
            try
            {
                var AllRequests = await membershipRequestRepository.GetAllRequestsByTenantId(tenantId);
                var request = mapper.Map<List<MembershipRequestDto>>(AllRequests);

                var countOfRequests = request.Count;

                var paginationValidationResult = ValidateOnPagination(pageSize, pageNumber);

                const string ascendingOrder = "ascending";

                const string descendingOrder = "descending";

                var sortedMembershipRequests = Enumerable.Empty<MembershipRequestDto>();

                // just searching without sorting
                if (!string.IsNullOrEmpty(newMembershipRequestsDto?.search) && (string.IsNullOrEmpty(newMembershipRequestsDto?.sort) || string.IsNullOrEmpty(newMembershipRequestsDto?.order)))
                {
                    var searchedMemberShipRequests = SearchHelper.Search(request.AsQueryable(), newMembershipRequestsDto.search);

                    var mappedMemberShipRequestsDtos = mapper.Map<List<MembershipRequestDto>>(searchedMemberShipRequests);

                    var countOfMemberShipRequestsDtos = mappedMemberShipRequestsDtos.Count;

                    var paginatedMemberShipRequestsDtos = mappedMemberShipRequestsDtos.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                    int totalPages = (int)Math.Ceiling(countOfMemberShipRequestsDtos / (double)paginationValidationResult.pageSize);

                    var result = new PaginationToReturnDto<MembershipRequestDto>(countOfMemberShipRequestsDtos, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedMemberShipRequestsDtos);

                    return new ResponseHandler().Success(result);
                }

                // just sorting without searching
                else if (!string.IsNullOrEmpty(newMembershipRequestsDto?.sort) && string.IsNullOrEmpty(newMembershipRequestsDto?.search))
                {

                    if (newMembershipRequestsDto?.order?.Replace(" ", "").ToLower() == ascendingOrder)
                    {
                        sortedMembershipRequests = SortingHelper.SortAscending<MembershipRequestDto, object>(request, newMembershipRequestsDto.sort);
                    }
                    else if (newMembershipRequestsDto?.order?.Replace(" ", "").ToLower() == descendingOrder)
                    {
                        sortedMembershipRequests = SortingHelper.SortDescending<MembershipRequestDto, object>(request, newMembershipRequestsDto.sort);
                    }
                    else
                        return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipRequestDto>>("Invalid Order");

                    var countOfSortedMemberShipRequestsDtos = sortedMembershipRequests.Count();

                    var paginatedSortedMemberShipRequestsDtos = sortedMembershipRequests.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                    int totalPages = (int)Math.Ceiling(countOfSortedMemberShipRequestsDtos / (double)paginationValidationResult.pageSize);

                    var result = new PaginationToReturnDto<MembershipRequestDto>(countOfSortedMemberShipRequestsDtos, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedSortedMemberShipRequestsDtos);

                    return new ResponseHandler().Success(result);
                }

                // searching and sorting
                else if (!string.IsNullOrEmpty(newMembershipRequestsDto?.search) && !string.IsNullOrEmpty(newMembershipRequestsDto?.sort))
                {
                    var searchedMembershipRequests = SearchHelper.Search(request.AsQueryable(), newMembershipRequestsDto.search);

                    var mappedSearchedMembershipRequests = mapper.Map<List<MembershipRequestDto>>(searchedMembershipRequests);

                    if (newMembershipRequestsDto?.order?.Replace(" ", "").ToLower() == ascendingOrder)
                    {
                        sortedMembershipRequests = SortingHelper.SortAscending<MembershipRequestDto, object>(mappedSearchedMembershipRequests, newMembershipRequestsDto.sort);
                    }
                    else if (newMembershipRequestsDto?.order?.Replace(" ", "").ToLower() == descendingOrder)
                    {
                        sortedMembershipRequests = SortingHelper.SortDescending<MembershipRequestDto, object>(mappedSearchedMembershipRequests, newMembershipRequestsDto.sort);
                    }
                    else
                        return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipRequestDto>>("Invalid Order");


                    var countOfSortedMembershipRequestsDtos = sortedMembershipRequests.Count();

                    var paginatedSortedMembershipRequestsDtos = sortedMembershipRequests.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                    int totalPages = (int)Math.Ceiling(countOfSortedMembershipRequestsDtos / (double)paginationValidationResult.pageSize);

                    var result = new PaginationToReturnDto<MembershipRequestDto>(countOfSortedMembershipRequestsDtos, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedSortedMembershipRequestsDtos);

                    return new ResponseHandler().Success(result);
                }
                else
                {
                    var paginatedRequests = request.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                    int totalPages = (int)Math.Ceiling(countOfRequests / (double)paginationValidationResult.pageSize);

                    var result = new PaginationToReturnDto<MembershipRequestDto>(countOfRequests, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedRequests);

                    return new ResponseHandler().Success(result);
                }

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipRequestDto>>(ex.Message);
            }
        }

        public async Task<Response<PaginationToReturnDto<MembershipRequestDto>>> GetAllPendingPaymentRequests(PendingPaymentRequestDto? pendingPaymentRequestDto, int tenantId, int pageSize = 5, int pageNumber = 1)
        {
            try
            {
                var filters = new Dictionary<string, string[]>
                {
                    { "Status", new[] { "PendingPayment" } }
                };

                var response = await membershipRequestRepository.GetAllRequestsByTenantId(tenantId);

                var membershipRequestDtos = mapper.Map<List<MembershipRequestDto>>(response);

                var filteredRequestDtos = FilterHelper.Filter(membershipRequestDtos, filters).ToList();

                var paginationValidationResult = ValidateOnPagination(pageSize, pageNumber);

                const string ascendingOrder = "ascending";

                const string descendingOrder = "descending";

                var sortedPendingRequests = Enumerable.Empty<MembershipRequestDto>();

                // just searching without sorting
                if (!string.IsNullOrEmpty(pendingPaymentRequestDto?.search) && (string.IsNullOrEmpty(pendingPaymentRequestDto?.sort) || string.IsNullOrEmpty(pendingPaymentRequestDto?.order)))
                {
                    var searchedPendingRequests = SearchHelper.Search(filteredRequestDtos.AsQueryable(), pendingPaymentRequestDto.search);

                    var mappedsearchedPendingRequestsDtos = mapper.Map<List<MembershipRequestDto>>(searchedPendingRequests);

                    var countOfsearchedPendingRequestsDtos = mappedsearchedPendingRequestsDtos.Count;

                    var paginatedSearchedPendingRequests = mappedsearchedPendingRequestsDtos.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                    int totalPages = (int)Math.Ceiling(countOfsearchedPendingRequestsDtos / (double)paginationValidationResult.pageSize);

                    var result = new PaginationToReturnDto<MembershipRequestDto>(countOfsearchedPendingRequestsDtos, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedSearchedPendingRequests);

                    return new ResponseHandler().Success(result);
                }
                // just sorting without searching
                else if (!string.IsNullOrEmpty(pendingPaymentRequestDto?.sort) && string.IsNullOrEmpty(pendingPaymentRequestDto?.search))
                {

                    if (pendingPaymentRequestDto?.order?.Replace(" ", "").ToLower() == ascendingOrder)
                    {
                        sortedPendingRequests = SortingHelper.SortAscending<MembershipRequestDto, object>(filteredRequestDtos, pendingPaymentRequestDto.sort);
                    }
                    else if (pendingPaymentRequestDto?.order?.Replace(" ", "").ToLower() == descendingOrder)
                    {
                        sortedPendingRequests = SortingHelper.SortDescending<MembershipRequestDto, object>(filteredRequestDtos, pendingPaymentRequestDto.sort);
                    }
                    else
                        return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipRequestDto>>("Invalid Order");


                    var countOfSortedPendingRequestsDtos = sortedPendingRequests.Count();

                    var paginatedSortedPendingRequests = sortedPendingRequests.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                    int totalPages = (int)Math.Ceiling(countOfSortedPendingRequestsDtos / (double)paginationValidationResult.pageSize);

                    var result = new PaginationToReturnDto<MembershipRequestDto>(countOfSortedPendingRequestsDtos, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedSortedPendingRequests);

                    return new ResponseHandler().Success(result);
                }
                // searching and sorting
                else if (!string.IsNullOrEmpty(pendingPaymentRequestDto?.search) && !string.IsNullOrEmpty(pendingPaymentRequestDto?.sort))
                {
                    var searchedPendingRequests = SearchHelper.Search(filteredRequestDtos.AsQueryable(), pendingPaymentRequestDto.search);

                    var mappedsearchedPendingRequestsDtos = mapper.Map<List<MembershipRequestDto>>(searchedPendingRequests);

                    if (pendingPaymentRequestDto?.order?.Replace(" ", "").ToLower() == ascendingOrder)
                    {
                        sortedPendingRequests = SortingHelper.SortAscending<MembershipRequestDto, object>(mappedsearchedPendingRequestsDtos, pendingPaymentRequestDto.sort);
                    }
                    else if (pendingPaymentRequestDto?.order?.Replace(" ", "").ToLower() == descendingOrder)
                    {
                        sortedPendingRequests = SortingHelper.SortDescending<MembershipRequestDto, object>(mappedsearchedPendingRequestsDtos, pendingPaymentRequestDto.sort);
                    }
                    else
                        return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipRequestDto>>("Invalid Order");


                    var countOfSortedPendingRequestsDtos = sortedPendingRequests.Count();

                    var paginatedSortedPendingRequests = sortedPendingRequests.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                    int totalPages = (int)Math.Ceiling(countOfSortedPendingRequestsDtos / (double)paginationValidationResult.pageSize);

                    var result = new PaginationToReturnDto<MembershipRequestDto>(countOfSortedPendingRequestsDtos, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedSortedPendingRequests);

                    return new ResponseHandler().Success(result);
                }
                else
                {
                    var countOfFilteredRequests = filteredRequestDtos.Count;

                    var paginatedFilteredRequests = filteredRequestDtos.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                    int totalPages = (int)Math.Ceiling(countOfFilteredRequests / (double)paginationValidationResult.pageSize);

                    var result = new PaginationToReturnDto<MembershipRequestDto>(countOfFilteredRequests, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedFilteredRequests);

                    return new ResponseHandler().Success(result);
                }

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipRequestDto>>(ex.Message);
            }
        }

        public async Task<Response<MembershipRequestDto>> GetMembershipRequestById(int tenantId, int requestId)
        {
            try
            {
                var request = await membershipRequestRepository.GetMembershipRequestsById(tenantId, requestId);
                var lastStatusHistory = request.RequestStatusHistories
                                          .OrderByDescending(h => h.ModificationDate)
                                          .Where(h => h.MembershipRequestId == requestId)
                                          .FirstOrDefault();


                var membershipRequestDto = mapper.Map<MembershipRequestDto>(request);
                double totalFees = CalculateTotalFees(membershipRequestDto);
                membershipRequestDto.FormFees = totalFees;
                var newMembershipReqDto = new MembershipRequestDto();

                if (membershipRequestDto.RequestType.Id == 9 || membershipRequestDto.RequestType.Id == 18 || membershipRequestDto.RequestType.Id == 16)
                {
                    membershipRequestDto.SubMembers[0].FormUserResponse = membershipRequestDto.FormUserResponse;
                    membershipRequestDto.FormUserResponse = null;
                    if (lastStatusHistory != null && lastStatusHistory.ModificationDate != null)
                    {
                        membershipRequestDto.SubMembers[0].LastStatusUpdate = lastStatusHistory.ModificationDate;
                    }
                    else if (lastStatusHistory != null)
                    {
                        membershipRequestDto.SubMembers[0].LastStatusUpdate = lastStatusHistory.CreationDate;
                    }

                    else
                    {
                        membershipRequestDto.SubMembers[0].LastStatusUpdate = request.CreationDate;
                    }


                }

                if (membershipRequestDto.RequestType.Id == 12 && membershipRequestDto.MemberTypeId != 5)
                {
                    var mainmembership = await membershipService.GeMembershipsById(request.Membership.ParentMembershipId.Value);
                    var mainmembershipDto = mainmembership.Data;
                    newMembershipReqDto = mapper.Map<MembershipRequestDto>(mainmembershipDto);
                    newMembershipReqDto.MembershipId = mainmembership.Data.Id;

                    newMembershipReqDto.SubMembers.Add(new SubMemberMembershipRequestDto());

                    newMembershipReqDto.Membership = mainmembershipDto;
                    newMembershipReqDto.MemberTypeId = 5;
                    newMembershipReqDto.MemberType = (await memberTypeService.GeTypeById(5)).Data;
                    newMembershipReqDto.SubMembers[0].FormUserResponse = membershipRequestDto.FormUserResponse;
                    newMembershipReqDto.SubMembers[0].FormUserResponse.SubMemberMembership = membershipRequestDto.Membership;
                    newMembershipReqDto.SubMembers[0].MemberTypeId = request.MemberTypeId;
                    newMembershipReqDto.SubMembers[0].MemberType = mapper.Map<MemberTypeDto>(request.MemberType);
                    newMembershipReqDto.FormUserResponse = null;
                    newMembershipReqDto.FormFees = totalFees;
                    newMembershipReqDto.Id = request.Id;
                    newMembershipReqDto.CreationDate = request.CreationDate;
                    newMembershipReqDto.Status = request.Status;
                    newMembershipReqDto.RequestType = membershipRequestDto.RequestType;
                    newMembershipReqDto.IsTermsAgreed = membershipRequestDto.IsTermsAgreed;
                    newMembershipReqDto.PackageId = membershipRequestDto.PackageId;

                    membershipRequestDto = newMembershipReqDto;

                    if (lastStatusHistory != null && lastStatusHistory.ModificationDate != null)
                    {
                        membershipRequestDto.SubMembers[0].LastStatusUpdate = lastStatusHistory.ModificationDate;
                    }
                    else if (lastStatusHistory != null)
                    {
                        membershipRequestDto.SubMembers[0].LastStatusUpdate = lastStatusHistory.CreationDate;
                    }

                    else
                    {
                        membershipRequestDto.SubMembers[0].LastStatusUpdate = request.CreationDate;
                    }


                }

                if (lastStatusHistory != null && lastStatusHistory.ModificationDate != null)
                {
                    membershipRequestDto.LastStatusUpdate = lastStatusHistory.ModificationDate;
                }
                else if (lastStatusHistory != null)
                {
                    membershipRequestDto.LastStatusUpdate = lastStatusHistory.CreationDate;
                }
                else
                {
                    membershipRequestDto.LastStatusUpdate = request.CreationDate;
                }

                membershipRequestDto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipRequestDto.NationalIdPhoto);
                membershipRequestDto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipRequestDto.UserPhoto);

                var tasks = membershipRequestDto.SubMembers.Select(async subMember =>
                {
                    subMember.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(subMember.NationalIdPhoto);
                    subMember.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(subMember.UserPhoto);
                });
                await Task.WhenAll(tasks);
                return new ResponseHandler().Success(membershipRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.Message);
            }
        }

        public async Task<Response<MembershipRequestDto>> UpdateRequestStatus(int tenantId, int requestId, RequestStatus newStatus)
        {
            try
            {
                // Retrieve the request by its ID
                var request = await unitOfWork.Repository<MembershipRequest>().GetByIdAsync(requestId);

                // Check if the tenant ID matches
                if (request.TenantId != tenantId)
                {
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Tenant ID does not match.");
                }

                // Update the request status
                request.Status = newStatus;

                var lastStatusHistory = new RequestStatusHistory
                {
                    MembershipRequestId = requestId,
                    RequestStatusId = newStatus,
                    ModificationDate = DateTime.Now,
                };

                request.RequestStatusHistories.Add(lastStatusHistory);
                // Update the specific field
                unitOfWork.Repository<MembershipRequest>().UpdateField(request, r => r.Status);
                await unitOfWork.Save();

                // Map the updated entity to a DTO
                var updatedRequestDto = mapper.Map<MembershipRequestDto>(request);
                return new ResponseHandler().Success(updatedRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<Response<MembershipRequestDto>> UpdateRequestStatusWithComment(UpdateRequestStatusWithComment model)
        {
            try
            {
                // Retrieve the request by its ID
                var request = await unitOfWork.Repository<MembershipRequest>().GetByIdAsync(model.requestId);

                // Check if the tenant ID matches
                if (request.TenantId != model.tenantId)
                {
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Tenant ID does not match.");
                }
                var payment = await unitOfWork.Repository<PaymentRequest>().GetByIdAsync(model.paymentId);
                if (payment is null)
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("there is no payment Request with this Id");
                // Update the request status
                request.Status = model.newStatus;

                // Update the specific field
                unitOfWork.Repository<MembershipRequest>().UpdateField(request, r => r.Status);
                await unitOfWork.Save();

                // SubmitPaymentComments
                await paymentRequestService.SubmitPaymentComments(model.tenantId, model.paymentId, model.comment);

                // Map the updated entity to a DTO
                var updatedRequestDto = mapper.Map<MembershipRequestDto>(request);
                return new ResponseHandler().Success(updatedRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<Response<MembershipRequestDto>> SubmitCommentsAndJustfications(SubmitCommentsAndJustficationsDto model)
        {
            try
            {
                // Retrieve the request by its ID
                var request = await unitOfWork.Repository<MembershipRequest>().GetByIdAsync(model.requestId);

                // Check if the tenant ID matches
                if (request.TenantId != model.tenantId)
                {
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Tenant ID does not match.");
                }
                var RequestComment = new MembershipRequestComment()
                {
                    MembershipRequestId = model.requestId,
                    CommentsAndJustfications = model.comments,
                    RequestStatus = request.Status,
                    CreatedBy = model.CreatedBy
                };
                await unitOfWork.Repository<MembershipRequestComment>().Add(RequestComment);
                await unitOfWork.Save();

                // Map the updated entity to a DTO
                var updatedRequestDto = mapper.Map<MembershipRequestDto>(request);
                return new ResponseHandler().Success(updatedRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<PaginationToReturnDto<MembershipRequestDto>>> SortMembershipRequests(string sortBy, string order, int pageSize = 5, int pageNumber = 1)
        {
            try
            {
                const string ascendingOrder = "ascending";

                const string descendingOrder = "descending";

                // Retrieve the requests
                var requests = await unitOfWork.Repository<MembershipRequest>().GetAllAsync();

                var sortedRequests = Enumerable.Empty<MembershipRequest>();

                if (order.Replace(" ", "").ToLower() == ascendingOrder)
                {
                    sortedRequests = SortingHelper.SortAscending<MembershipRequest, object>(requests, sortBy);
                }
                else if (order.Replace(" ", "").ToLower() == descendingOrder)
                {
                    sortedRequests = SortingHelper.SortDescending<MembershipRequest, object>(requests, sortBy);
                }
                else
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipRequestDto>>("Invalid Order");

                // Map the sorted entities to DTOs
                var sortedRequestDtos = mapper.Map<List<MembershipRequestDto>>(sortedRequests);

                var tasks = sortedRequestDtos.Select(async dto =>
                {
                    dto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.NationalIdPhoto);
                    dto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.UserPhoto);



                });

                var subMemberTasks = sortedRequestDtos.SelectMany(dto => dto.SubMembers).Select(async subDto =>
                 {
                     subDto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(subDto.NationalIdPhoto);
                     subDto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(subDto.UserPhoto);
                 });

                await Task.WhenAll(tasks.Concat(subMemberTasks).ToList());

                var countOfSortedRequests = sortedRequestDtos.Count;

                var paginationValidationResult = ValidateOnPagination(pageSize, pageNumber);

                var paginatedSortedRequests = sortedRequestDtos.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                int totalPages = (int)Math.Ceiling(countOfSortedRequests / (double)paginationValidationResult.pageSize);

                var result = new PaginationToReturnDto<MembershipRequestDto>(countOfSortedRequests, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedSortedRequests);


                return new ResponseHandler().Success(result);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipRequestDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<Response<PaginationToReturnDto<MembershipRequestDto>>> GetFilteredTenantRequests(int tenantId, int requestStatus, int pageSize = 5, int pageNumber = 1)
        {
            try
            {
                // Convert the status filter integer to the corresponding enum name

                string statusName = Enum.GetName(typeof(RequestStatus), requestStatus);
                var filters = new Dictionary<string, string[]>
                {
                    { "Status", new[] { statusName } }
                };

                var response = await membershipRequestRepository.GetAllRequestsByTenantId(tenantId);

                var filteredRequests = FilterHelper.Filter(response, filters).ToList();

                var filteredRequestsDto = mapper.Map<List<MembershipRequestDto>>(filteredRequests);


                var tasks = filteredRequestsDto.Select(async dto =>
                {
                    dto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.NationalIdPhoto);
                    dto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.UserPhoto);



                });

                var subMemberTasks = filteredRequests.SelectMany(dto => dto.SubMembers).Select(async subDto =>
                {
                    subDto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(subDto.NationalIdPhoto);
                    subDto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(subDto.UserPhoto);
                });

                await Task.WhenAll(tasks.Concat(subMemberTasks).ToList());

                var countOfFilteredRequests = filteredRequestsDto.Count;

                var paginationValidationResult = ValidateOnPagination(pageSize, pageNumber);

                var paginatedFilteredRequests = filteredRequestsDto.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                int totalPages = (int)Math.Ceiling(countOfFilteredRequests / (double)paginationValidationResult.pageSize);

                var result = new PaginationToReturnDto<MembershipRequestDto>(countOfFilteredRequests, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedFilteredRequests);

                return new ResponseHandler().Success(result);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipRequestDto>>(ex.Message);
            }
        }
        public async Task<Response<PaginationToReturnDto<MembershipRequestDto>>> SearchMembershipRequests(string searchTerm, int pageSize = 5, int pageNumber = 1)
        {
            try
            {
                // Retrieve the requests
                var requests = await unitOfWork.Repository<MembershipRequest>().GetAllAsync();

                // Perform the search
                var searchedRequests = SearchHelper.Search(requests.AsQueryable(), searchTerm);

                // Map the searched entities to DTOs
                var searchedRequestsDtos = mapper.Map<List<MembershipRequestDto>>(searchedRequests.ToList());


                var countOfSearchedRequests = searchedRequestsDtos.Count;

                var paginationValidationResult = ValidateOnPagination(pageSize, pageNumber);

                var paginatedSearchedRequests = searchedRequestsDtos.Skip((paginationValidationResult.pageNumber - 1) * paginationValidationResult.pageSize).Take(paginationValidationResult.pageSize).ToList();

                var tasks = paginatedSearchedRequests.Select(async dto =>
                {
                    dto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.NationalIdPhoto);
                    dto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.UserPhoto);



                });

                var subMemberTasks = paginatedSearchedRequests.SelectMany(dto => dto.SubMembers).Select(async subDto =>
                {
                    subDto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(subDto.NationalIdPhoto);
                    subDto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(subDto.UserPhoto);
                });

                await Task.WhenAll(tasks.Concat(subMemberTasks).ToList());

                int totalPages = (int)Math.Ceiling(countOfSearchedRequests / (double)paginationValidationResult.pageSize);

                var result = new PaginationToReturnDto<MembershipRequestDto>(countOfSearchedRequests, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, paginatedSearchedRequests);

                return new ResponseHandler().Success(result);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipRequestDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private (int pageSize, int pageNumber) ValidateOnPagination(int pageSize, int pageNumber)
        {

            if (pageSize > 5 || pageSize < 1)
                pageSize = 5;

            if (pageNumber < 1)
                pageNumber = 1;

            return (pageSize, pageNumber);
        }

        public async Task<Response<MembershipRequestDto>> UpdateParentWithSubmembersRequestStatus(int tenantId, int requestId)
        {
            try
            {
                // Retrieve the request by its ID
                var request = await membershipRequestRepository.GetMembershipRequestsById(tenantId, requestId);

                // Check if the tenant ID matches
                if (request == null || request.TenantId != tenantId)
                {
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Request not found or tenant ID does not match.");
                }

                // Ensure Membership is not null
                if (request.Membership == null)
                {
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Membership is not initialized.");
                }

                // Update the parent request status and membership activation
                request.Status = RequestStatus.RequestResolved;
                request.Membership.MembershipStatus = MembershipStatus.Active;

                // Ensure RequestStatusHistories is initialized
                if (request.RequestStatusHistories == null)
                {
                    request.RequestStatusHistories = new List<RequestStatusHistory>();
                }

                // Check if a status history with the same keys already exists
                var existingParentStatusHistory = request.RequestStatusHistories
                    .FirstOrDefault(rsh => rsh.MembershipRequestId == requestId && rsh.RequestStatusId == RequestStatus.RequestResolved);

                if (existingParentStatusHistory == null)
                {
                    var parentStatusHistory = new RequestStatusHistory
                    {
                        MembershipRequestId = requestId,
                        RequestStatusId = RequestStatus.RequestResolved,
                        ModificationDate = DateTime.Now,
                    };
                    request.RequestStatusHistories.Add(parentStatusHistory);
                }

                // Update each submember's request status and membership activation
                if (request.SubMembers != null)
                {
                    foreach (var subMemberRequest in request.SubMembers)
                    {
                        subMemberRequest.Status = RequestStatus.RequestResolved;
                        subMemberRequest.Membership.MembershipStatus = MembershipStatus.Active;

                        // Check if a status history with the same keys already exists for the submember
                        var existingSubMemberStatusHistory = subMemberRequest.RequestStatusHistories
                            .FirstOrDefault(rsh => rsh.MembershipRequestId == subMemberRequest.Id && rsh.RequestStatusId == RequestStatus.RequestResolved);

                        if (existingSubMemberStatusHistory == null)
                        {
                            var subMemberStatusHistory = new RequestStatusHistory
                            {
                                MembershipRequestId = subMemberRequest.Id, // Use the submember request ID
                                RequestStatusId = RequestStatus.RequestResolved,
                                ModificationDate = DateTime.Now,
                            };
                            subMemberRequest.RequestStatusHistories.Add(subMemberStatusHistory);
                        }

                        // Update submember request
                        unitOfWork.Repository<MembershipRequest>().Update(subMemberRequest);
                    }
                }

                // Update the parent request
                unitOfWork.Repository<MembershipRequest>().Update(request);

                // Save all changes
                await unitOfWork.Save();

                // Map the updated entity to a DTO
                var updatedRequestDto = mapper.Map<MembershipRequestDto>(request);
                updatedRequestDto.LastStatusUpdate = DateTime.Now;
                return new ResponseHandler().Success(updatedRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<MembershipRequestDto>> UpdateRequestAndMembershipStatus(int tenantId, int requestId, RequestStatus newStatus)
        {
            try
            {
                // Retrieve the request by its ID
                var request = await membershipRequestRepository.GetMembershipRequestsById(tenantId, requestId);

                // Check if the tenant ID matches
                if (request.TenantId != tenantId)
                {
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Tenant ID does not match.");
                }

                // Execute the logic based on the request type
                var executeLogicResult = await formService.ExecuteLogicForRequestType(new ExecuteLogicForRequestTypeDto
                {
                    RequestTypeId = request.RequestType.Id, // assuming model.requestTypeId is available
                    FormUserResponseId = request.FormUserResponse.Id, // assuming this is available in the model
                    SubMembershipRequestsId = request.SubMembers.Select(s => s.Id).ToList()  // assuming this is available in the model
                });



                if (!executeLogicResult.Succeeded)
                {
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Failed to execute logic for the request type.");
                }


                // Update the request status
                request.Status = newStatus;

                // Check if a status history with the same keys is already tracked
                var existingHistory = request.RequestStatusHistories
                    .FirstOrDefault(h => h.MembershipRequestId == requestId && h.RequestStatusId == newStatus);

                if (existingHistory == null)
                {
                    // Add a new entry to the request status history
                    var lastStatusHistory = new RequestStatusHistory
                    {
                        MembershipRequestId = requestId,
                        RequestStatusId = newStatus,
                        ModificationDate = DateTime.Now,
                    };

                    request.RequestStatusHistories.Add(lastStatusHistory);
                }
                else
                {
                    // Optionally update the existing status history
                    existingHistory.ModificationDate = DateTime.Now;
                    unitOfWork.Repository<MembershipRequest>().UpdateField(request, r => r.ModificationDate);

                }


                // Update the request in the repository
                unitOfWork.Repository<MembershipRequest>().UpdateField(request, r => r.Status);
                var package = await membershipService.GetPackageByIdAsync(request.PackageId.Value);
                if (request.RequestTypeId == 6)
                {
                    var payment = new PaymentRequest
                    {
                        CreatedBy = request.CreatedBy,
                        ServiceCharges = 100,
                        MembershipFees = package.Price,
                        Taxes = (100 + package.Price) * 0.14,
                        TotalAmount = 100 + package.Price + (100 + package.Price) * 0.14,
                        MembershipRequestId = requestId,
                        TenantId = request.TenantId,
                        RequestStatus = RequestStatus.Pending, // Example status, update as needed
                        Comments = "Initial payment request" // Optional, can be null or any starting comment
                    };

                    // Add the new payment to the PaymentRequests collection
                    request.PaymentRequests.Add(payment);

                    // Use the Unit of Work to update the MembershipRequest entity
                    unitOfWork.Repository<MembershipRequest>().Update(request);
                }

                // Save all changes
                await unitOfWork.Save();

                // Return a successful response with the updated request
                var membershipRequestDto = new MembershipRequestDto
                {
                    // Populate the DTO with the necessary properties from the request entity
                    Id = request.Id,
                    Status = request.Status,
                    MembershipId = request?.MembershipId
                    // Add other properties if needed
                };

                return new ResponseHandler().Success(membershipRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<MembershipRequestDto>>> GeMembershipRequestssByPackageId(int PackageId)
        {
            try
            {
                var memberships = await membershipRequestRepository.GeMembershipRequestByPackageId(PackageId);
                var membershipDtos = mapper.Map<List<MembershipRequestDto>>(memberships);

                var tasks = membershipDtos.Select(async dto =>
                {
                    dto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.NationalIdPhoto);
                    dto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.UserPhoto);
                });

                var subMemberTasks = membershipDtos.SelectMany(dto => dto.SubMembers).Select(async subDto =>
                {
                    subDto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(subDto.NationalIdPhoto);
                    subDto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(subDto.UserPhoto);
                });
                await Task.WhenAll(tasks.Concat(subMemberTasks).ToList());

                return new ResponseHandler().Success(membershipDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MembershipRequestDto>>(ex.Message);
            }
        }
        public async Task<Response<object>> GetRevenuesPerMembershipsPackage(int tenantId, int month, int year)
        {
            try
            {
                var dateTime = new DateTime(year, month, 1);
                var lastDayInMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

                // Get all membership requests for the tenant
                var membershipRequests = await membershipRequestRepository.GetAllRequestsByTenantId(tenantId);

                if (membershipRequests.Count == 0)
                    return new ResponseHandler().Success<object>(new { MembershipRevenue = new Dictionary<string, List<object>>() });

                // Filter and organize payment requests for the specified month and year
                membershipRequests = membershipRequests.Select(p =>
                {
                    p.PaymentRequests = p.PaymentRequests
                        .Where(pp => pp.CreationDate.HasValue &&
                                     pp.CreationDate.Value.Month == month &&
                                     pp.CreationDate.Value.Year == year)
                        .ToList();
                    return p;
                }).ToList();

                // Check if there are any payments for the specified month and year
                var groupedPaymentRequests = membershipRequests
                    .SelectMany(p => p.PaymentRequests, (p, pp) => new { p.PackageId, PaymentRequest = pp })
                    .GroupBy(x => new { x.PackageId, x.PaymentRequest.CreationDate.Value.Day })
                    .Select(g => new
                    {
                        g.Key.PackageId,
                        g.Key.Day,
                        PaymentRequestsCount = g.Count(),
                        TotalRevenue = g.Sum(p => p.PaymentRequest.TotalAmount) / 1000
                    })
                    .ToList();

                if (groupedPaymentRequests.Count == 0)
                {
                    // No data for this month and year, return empty DTO
                    return new ResponseHandler().Success<object>(new
                    {
                        MinRevenue = 0,
                        Interval = 50,
                        MaxRevenue = 500,
                        MembershipRevenue = new Dictionary<string, List<object>>()
                    });
                }

                int maxRevenue = Convert.ToInt32(groupedPaymentRequests.Max(g => g.TotalRevenue));

                // Retrieve package names for each unique PackageId
                var packageNames = new Dictionary<int, string>();
                foreach (var packageId in membershipRequests.Select(r => r.PackageId).Distinct())
                {
                    var package = await membershipService.GetPackageByIdAsync(packageId.Value);
                    packageNames[packageId.Value] = package?.Name ?? "Unknown Package";
                }

                // Build the final data dictionary using package names
                var data = new Dictionary<string, List<object>>();

                foreach (var packageId in packageNames.Keys)
                {
                    var packageName = packageNames[packageId];
                    data[packageName] = new List<object>();

                    // Loop through each day in the month to fill in the daily revenue data
                    for (int dayIndex = 1; dayIndex <= lastDayInMonth; dayIndex++)
                    {
                        var programWithSpecificDay = groupedPaymentRequests
                            .FirstOrDefault(g => g.PackageId == packageId && g.Day == dayIndex);

                        if (programWithSpecificDay is not null)
                            data[packageName].Add(programWithSpecificDay);
                        else
                            data[packageName].Add(new { PackageId = packageId, Day = dayIndex, ProgramSubscribersCount = 0, TotalRevenue = 0 });
                    }
                }

                var resultToReturnDto = new
                {
                    MinRevenue = 0,
                    Interval = 50,
                    MaxRevenue = maxRevenue > 450 ? maxRevenue + 50000 : 500,
                    MembershipRevenue = data
                };

                return new ResponseHandler().Success<object>(resultToReturnDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<object>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<MembershipRequestDto>> ApproveRequestAndCreateMembership(int requestId)
        {
            try
            {


                // Step 1: Get the membership request
                var specification = new GeneralSpecifications<MembershipRequest>(
                           r => r.Id == requestId)
                {
                    Includes = new List<Func<IQueryable<MembershipRequest>, IQueryable<MembershipRequest>>>
                        {
                            q => q.Include(r => r.RequestType),
                            q => q.Include(r => r.FormUserResponse)
                                  .ThenInclude(fur => fur.FieldsResponse)
                                      .ThenInclude(fr => fr.Field),
                            q => q.Include(r => r.RequestStatusHistories),
                            q => q.Include(r => r.RequestType),
                            q => q.Include(r => r.Tenant),
                            q => q.Include(r => r.SuperTenant)


                        }
                };

                var request = await unitOfWork.Repository<MembershipRequest>().GetWithSpecifications(specification);
                if (request == null)
                    return new ResponseHandler().NotFound<MembershipRequestDto>("Membership request not found.");


                var claims = httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet();

                if (claims != null && claims.Contains("BranchAdmin"))
                {
                    if (request.BranchAdminStatus != BranchAdminStatus.Pending)
                    {
                        return new ResponseHandler().BadRequest<MembershipRequestDto>("request is not Pindding");
                    }
                    request.BranchAdminStatus = BranchAdminStatus.Approved;
                    unitOfWork.Repository<MembershipRequest>().Update(request);
                    await unitOfWork.Save();

                }

                else if (claims != null && claims.Contains("TenantAdmin"))
                {
                    if (request.Status != RequestStatus.Pending)
                    {
                        return new ResponseHandler().BadRequest<MembershipRequestDto>("request is not Pindding");
                    }
                    request.Status = RequestStatus.RequestResolved;
                    unitOfWork.Repository<MembershipRequest>().Update(request);
                    // Step 2: Find associated payment request
                    var paymentRequests = await unitOfWork.Repository<NewPaymentRequest>()
                         .FindAllByCriteriaAsync(pr => pr.MembershipRequestId == requestId);


                    var lastRequest = paymentRequests
                        .OrderByDescending(pr => pr.Id)
                        .FirstOrDefault();


                    unitOfWork.Repository<MembershipRequest>().Update(request);

                    var userData = new DTOs.Membership.UserDtos.UserDataDto();

                    if (lastRequest != null && lastRequest.PaymentStatus == PaymentStatus.Confirmed)
                    {
                        var response = await CheckOutMembership(formService, unitOfWork, request, requestId);
                        if (!response.Succeeded)
                            return new ResponseHandler().BadRequest<MembershipRequestDto>(response.Message);

                        userData.Users.Add(new UserInfoDto { UserId = request.UserId, BeltName = request.BeltName, UserName = $"{request.FirstName} {request.MiddleName} {request.LastName}" });

                    }
                    await unitOfWork.Save();

                    SetUserData(userData, request);

                    backgroundJobClient.Enqueue<MyEntityMembershipService.Services.Membership.PublishService.PublishService>(
                        x => x.PublishUserData(userData)
                    );

                }

                var membershipRequestDto = new MembershipRequestDto
                {
                    Id = request.Id,
                    Status = request.Status,
                    MembershipId = request.MembershipId
                };

                return new ResponseHandler().Success(membershipRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<Response<MembershipRequestDto>> ApproveRequestAndTransferOrLonedMembership(int requestId)
        {
            try
            {
                Log.Information("Starting ApproveRequestAndTransferOrLonedMembership for RequestId: {RequestId}", requestId);

                await unitOfWork.BeginTransactionAsync();
                Log.Information("Transaction started");
                // Step 1: Get the membership request
                var specification = new GeneralSpecifications<MembershipRequest>(
                           r => r.Id == requestId)
                {
                    Includes = new List<Func<IQueryable<MembershipRequest>, IQueryable<MembershipRequest>>>
                        {
                            q => q.Include(r => r.RequestType),
                            q => q.Include(r => r.FormUserResponse)
                                  .ThenInclude(fur => fur.FieldsResponse)
                                      .ThenInclude(fr => fr.Field),
                            q => q.Include(r => r.RequestStatusHistories),
                            q => q.Include(r => r.RequestType),
                              q => q.Include(r => r.Tenant),
                            q => q.Include(r => r.SuperTenant)


                        }
                };

                var request = await unitOfWork.Repository<MembershipRequest>().GetWithSpecifications(specification);
                if (request == null)
                {
                    Log.Warning("Membership request not found for RequestId: {RequestId}", requestId);
                    return new ResponseHandler().NotFound<MembershipRequestDto>("Membership request not found.");
                }

                Log.Warning("Membership request not found for RequestId: {RequestId}", requestId);
                var claims = httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet();


                if (claims != null && claims.Contains("BranchAdmin"))
                {
                    Log.Information("Current user is BranchAdmin");
                    if (request.BranchAdminStatus != BranchAdminStatus.Pending)
                    {
                        Log.Warning("BranchAdmin attempted to approve a request with non-pending status");
                        return new ResponseHandler().BadRequest<MembershipRequestDto>("request is not Pindding");
                    }
                    request.BranchAdminStatus = BranchAdminStatus.Approved;
                    unitOfWork.Repository<MembershipRequest>().Update(request);
                    Log.Information("BranchAdminStatus updated to Approved");
                }

                else if (claims != null && claims.Contains("TenantAdmin"))
                {
                    Log.Information("Current user is TenantAdmin");
                    if (request.Status != RequestStatus.Pending)
                    {
                        Log.Warning("TenantAdmin attempted to approve a request with non-pending status");
                        return new ResponseHandler().BadRequest<MembershipRequestDto>("request is not Pindding");
                    }
                    request.Status = RequestStatus.RequestResolved;
                    unitOfWork.Repository<MembershipRequest>().Update(request);
                    Log.Information("Request status updated to RequestResolved");


                    // Step 2: Find associated payment request
                    var paymentRequest = await unitOfWork.Repository<NewPaymentRequest>()
                        .FindAsync(pr => pr.MembershipRequestId == requestId);


                    unitOfWork.Repository<MembershipRequest>().Update(request);

                    // Check if the payment is confirmed
                    if (paymentRequest != null && paymentRequest.PaymentStatus == PaymentStatus.Confirmed)
                    {
                        var oldmembership = await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Membership>().FindByCriteriaAsync(r =>
                            r.Id == request.MembershipId &&
                            r.TenantId == request.FromTenantId);

                        if (oldmembership.MembershipStatus == MembershipStatus.Transfered || oldmembership.MembershipStatus == MembershipStatus.Removed)
                        {
                            return new ResponseHandler().BadRequest<MembershipRequestDto>("Membership is Transfered or Removed already.");
                        }

                        // Update old membership status
                        if (request.RequestTypeId == 19)
                        {
                            oldmembership.MembershipStatus = MembershipStatus.Transfered;
                            Log.Information("Old membership marked as Transfered");
                        }
                        else if (request.RequestTypeId == 20)
                        {
                            oldmembership.MembershipStatus = MembershipStatus.Loaned;
                            Log.Information("Old membership marked as Loaned");
                        }
                        else
                        {

                            Log.Information("Old membership doesn't marked");
                        }

                        unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Membership>().Update(oldmembership);

                        // Check if a non-active membership exists for the same user in the target tenant
                        var existingMembershipInTargetTenant = await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Membership>().FindByCriteriaAsync(r =>
                            r.UserId == oldmembership.UserId &&
                            r.TenantId == request.TenantId &&
                            r.MembershipStatus != MembershipStatus.Active);

                        if (existingMembershipInTargetTenant != null)
                        {
                            Log.Information("Existing non-active membership found in target tenant. Reactivating it");

                            // Reactivate existing membership
                            existingMembershipInTargetTenant.MembershipStatus = MembershipStatus.Active;
                            existingMembershipInTargetTenant.CreationDate = DateTime.Now;
                            existingMembershipInTargetTenant.CreatedBy = request.CreatedBy;

                            unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Membership>().Update(existingMembershipInTargetTenant);
                        }
                        else
                        {
                            Log.Information("No existing membership found in target tenant. Creating a new one");

                            // Create a new membership
                            var newMembership = new MyEntityMembershipService.Data.Membership.Entities.Membership
                            {
                                FirstName = oldmembership.FirstName,
                                LastName = oldmembership.LastName,
                                StartDate = oldmembership.StartDate,
                                EndDate = oldmembership.EndDate,
                                MembershipType = oldmembership.MembershipType,
                                NationalId = oldmembership.NationalId,
                                Nationality = oldmembership.Nationality,
                                NationalIDPhoto = oldmembership.NationalIDPhoto,
                                UserPhoto = oldmembership.UserPhoto,
                                BirthDate = oldmembership.BirthDate,
                                PhoneNumber = oldmembership.PhoneNumber,
                                Gender = oldmembership.Gender,
                                Address = oldmembership.Address,
                                Country = oldmembership.Country,
                                City = oldmembership.City,
                                Governate = oldmembership.Governate,
                                PackageId = oldmembership.PackageId,
                                FileNumber = oldmembership.FileNumber,
                                Email = oldmembership.Email,
                                UserId = oldmembership.UserId,
                                MemberTypeId = oldmembership.MemberTypeId,
                                ParentMembershipId = oldmembership.ParentMembershipId,
                                MemberShipId = Guid.NewGuid().ToString(),
                                BeltName = oldmembership.BeltName,

                                // New membership values
                                MembershipStatus = MembershipStatus.Active,
                                TenantId = request.TenantId,
                                CreatedBy = request.CreatedBy,
                                CreationDate = DateTime.Now
                            };

                            await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Membership>().Add(newMembership);
                            Log.Information("New membership created successfully for user: {UserId}", newMembership.UserId);

                        }
                    }


                }



                await unitOfWork.Save();
                Log.Information("Changes saved successfully");

                var membershipRequestDto = new MembershipRequestDto
                {
                    Id = request.Id,
                    Status = request.Status,
                    MembershipId = request.MembershipId
                };
                await unitOfWork.CommitTransactionAsync();

                Log.Information("Transaction committed successfully for RequestId: {RequestId}", request.Id);

                return new ResponseHandler().Success(membershipRequestDto);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();
                Log.Error(ex, "Error in ApproveRequestAndTransferOrLonedMembership method");

                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }


        public async Task<Response<MembershipRequestDto>> ApproveRequestStatus(int requestId)
        {
            try
            {

                // Retrieve the request by its ID
                var request = await unitOfWork.Repository<MembershipRequest>().GetByIdAsync(requestId);
                var claims = httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet();

                if (request.Status == RequestStatus.RequestResolved)
                {
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Payment request is already approved");
                }
                if (claims != null && claims.Contains("BranchAdmin"))
                {
                    request.BranchAdminStatus = BranchAdminStatus.Approved;
                    unitOfWork.Repository<MembershipRequest>().Update(request);

                }

                else if (claims != null && claims.Contains("TenantAdmin"))
                {
                    request.Status = RequestStatus.RequestResolved;
                    unitOfWork.Repository<MembershipRequest>().Update(request);

                }
                await unitOfWork.Save();

                var membershipRequestDto = new MembershipRequestDto
                {
                    Id = request.Id,
                    Status = request.Status
                };

                return new ResponseHandler().Success(membershipRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<MembershipRequestDto>> RejectRequestStatus(int requestId, RejectionReason rejectionReason)
        {
            try
            {
                // Retrieve the request by its ID
                var request = await unitOfWork.Repository<MembershipRequest>().GetByIdAsync(requestId);

                var claims = httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet();

                if (request.Status == RequestStatus.Rejected)
                {
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Payment request is already Rejected");
                }
                if (claims != null && claims.Contains("BranchAdmin"))
                {
                    request.BranchAdminStatus = BranchAdminStatus.Rejected;
                    unitOfWork.Repository<MembershipRequest>().Update(request);

                }

                else if (claims != null && claims.Contains("TenantAdmin"))
                {
                    request.Status = RequestStatus.Rejected;
                    request.RejectionReason = rejectionReason;
                    unitOfWork.Repository<MembershipRequest>().Update(request);

                }

                await unitOfWork.Save();

                var membershipRequestDto = new MembershipRequestDto
                {
                    Id = request.Id,
                    Status = request.Status
                };

                return new ResponseHandler().Success(membershipRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }



        // Method to calculate fees from the main request
        public double CalculateFeesFromMainRequest(MembershipRequestDto mainRequest)
        {
            if (mainRequest.FormUserResponse == null || mainRequest.FormUserResponse.FormFees == null)
            {
                return 0; // Return 0 if no form or fees are available
            }

            return mainRequest.FormUserResponse.FormFees.Value; // Get fees from the main request
        }

        // Method to calculate fees from submember requests
        public double CalculateFeesFromSubMemberRequests(MembershipRequestDto mainRequest)
        {
            if (mainRequest.SubMembers == null || !mainRequest.SubMembers.Any())
            {
                return 0; // Return 0 if no submembers exist
            }

            // Sum up the fees from all submember requests
            return mainRequest.SubMembers
                .Where(sub => sub.FormUserResponse?.FormFees != null) // Filter submembers with valid form fees
                .Sum(sub => sub.FormUserResponse.FormFees.Value);
        }

        // Method to calculate the total fees
        public double CalculateTotalFees(MembershipRequestDto mainRequest)
        {
            double mainRequestFees = CalculateFeesFromMainRequest(mainRequest);
            double subMemberFees = CalculateFeesFromSubMemberRequests(mainRequest);

            return mainRequestFees + subMemberFees; // Return the sum of all fees
        }

        public async static Task<Response<object>> CheckOutMembership(IFormService formService, IUnitOfWork unitOfWork, MembershipRequest request, int requestId)
        {
            // Step 4: Create Membership
            var executeLogicResult = await formService.ExecuteLogicForRequestType(new ExecuteLogicForRequestTypeDto
            {
                RequestTypeId = 6,
                FormUserResponseId = request.FormUserResponse.Id,
            });

            if (!executeLogicResult.Succeeded)
            {
                return new ResponseHandler().BadRequest<object>("Failed to execute logic for the request type.");
            }
            //step:5 Update Requsrt status History

            var existingHistory = request.RequestStatusHistories
          .FirstOrDefault(h => h.MembershipRequestId == requestId && h.RequestStatusId == RequestStatus.RequestResolved);

            if (existingHistory == null)
            {
                request.RequestStatusHistories.Add(new RequestStatusHistory
                {
                    MembershipRequestId = requestId,
                    RequestStatusId = RequestStatus.RequestResolved,
                    ModificationDate = DateTime.Now,
                });
            }
            else
            {
                existingHistory.ModificationDate = DateTime.Now;
                unitOfWork.Repository<MembershipRequest>().UpdateField(request, r => r.ModificationDate);
            }
            return new ResponseHandler().Success<object>("Done");

        }
        public async Task<Response<PagedResult<ConfirmedMembershipRequestDto>>> GetConfirmedMembershipsAsync(int entityId, int pageIndex, int pageSize)
        {
            try
            {
                var specification = new GeneralSpecifications<MembershipRequest>(
                    req =>
                        req.TenantId == entityId &&
                        req.Status == RequestStatus.RequestResolved &&
                        req.NewPaymentRequests != null &&
                        req.NewPaymentRequests.All(p => p.PaymentStatus == PaymentStatus.Confirmed) &&
                        req.ModificationDate.HasValue,
                    pageIndex,
                    pageSize
                )
                {
                    Includes = new List<Func<IQueryable<MembershipRequest>, IQueryable<MembershipRequest>>>
            {
                q => q.Include(r => r.NewPaymentRequests)
            }
                };

                var requests = await unitOfWork.Repository<MembershipRequest>()
                    .GetAllWithSpecifications(specification);

                var totalCount = await unitOfWork.Repository<MembershipRequest>()
                    .GetCountByCriteriaAsync(req =>
                        req.TenantId == entityId &&
                        req.Status == RequestStatus.RequestResolved &&
                        req.NewPaymentRequests != null &&
                        req.NewPaymentRequests.All(p => p.PaymentStatus == PaymentStatus.Confirmed) &&
                        req.ModificationDate.HasValue
                    );

                var result = requests.Select(req =>
                {
                    var modificationDate = req.ModificationDate.Value;
                    var expiryDate = modificationDate.AddMonths(12);

                    return new ConfirmedMembershipRequestDto
                    {
                        FullName = $"{req.FirstName} {req.LastName}",
                        MembershipId = $"MPM{req.Id}",
                        PhoneNumber = req.PhoneNumber,
                        BeltName = req.BeltName,
                        MemberSince = modificationDate.ToString("yyyy-MM-dd"),
                        MembershipStartDate = modificationDate.ToString("yyyy-MM-dd"),
                        MembershipExpiryDate = expiryDate.ToString("yyyy-MM-dd"),
                        MembershipStatus = expiryDate > DateTime.UtcNow ? "Active" : "Expired"
                    };
                }).ToList();

                var pagedResult = new PagedResult<ConfirmedMembershipRequestDto>
                {
                    Items = result,
                    PageNumber = pageIndex,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return new ResponseHandler().Success(pagedResult);
            }
            catch (Exception)
            {
                return new ResponseHandler().BadRequest<PagedResult<ConfirmedMembershipRequestDto>>("Failed to retrieve confirmed memberships.");
            }
        }

        public async Task<Response<EntityMembershipAnalyticsDto>> GetEntityMembershipAnalytics(int tenantId)
        {
            try
            {
                var specification = new GeneralSpecifications<MembershipRequest>(
                    req =>
                        req.TenantId == tenantId &&
                        req.Status == RequestStatus.RequestResolved &&
                        req.ModificationDate.HasValue &&
                        req.NewPaymentRequests != null &&
                        req.NewPaymentRequests.All(p => p.PaymentStatus == PaymentStatus.Confirmed)
                )
                {
                    Includes = new List<Func<IQueryable<MembershipRequest>, IQueryable<MembershipRequest>>>
                        {
                            q => q.Include(r => r.NewPaymentRequests),
                             q => q.Include(r=>r.Tenant)
                        }
                };

                var requests = await unitOfWork.Repository<MembershipRequest>()
                    .GetAllWithSpecifications(specification);

                int total = requests.Count;
                int active = requests.Count(r => r.ModificationDate.Value.AddMonths(12) > DateTime.UtcNow.Date);
                int expired = total - active;

                string creationDate = requests.Any()
              ? requests.Min(r => r.Tenant.CreationDate.Value).ToString("yyyy-MM-dd")
              : DateTime.UtcNow.ToString("yyyy-MM-dd");

                var result = new EntityMembershipAnalyticsDto
                {
                    TotalMembers = total,
                    ActiveMembers = active,
                    ExpiredMembers = expired,
                    CreationDate = creationDate
                };

                return new ResponseHandler().Success(result);
            }
            catch (Exception)
            {
                return new ResponseHandler().BadRequest<EntityMembershipAnalyticsDto>("Failed to load analytics.");
            }
        }

        public async Task<Response<string>> SoftDeleteMembershipRequestAsync(int requestId)
        {
            try
            {
                var request = await unitOfWork.Repository<MembershipRequest>()
                    .GetByIdAsync(requestId);

                if (request == null)
                    return new ResponseHandler().BadRequest<string>("Membership request not found.");

                // Soft delete
                request.IsDeleted = true;

                unitOfWork.Repository<MembershipRequest>().Update(request);

                // Step 2: Check if the request has a valid BatchId
                if (request.BatchId.HasValue)
                {
                    int batchId = request.BatchId.Value;

                    // Count remaining non-deleted requests in the batch (excluding this one)
                    var remainingRequests = await unitOfWork.Repository<MembershipRequest>()
                        .GetCountByCriteriaAsync(r => r.BatchId == batchId && !r.IsDeleted && r.Id != requestId);

                    // If no other requests remain, soft delete the batch
                    if (remainingRequests == 0)
                    {
                        var batch = await unitOfWork.Repository<Batch>()
                            .GetByIdAsync(batchId);

                        if (batch != null)
                        {
                            batch.IsDeleted = true;
                            unitOfWork.Repository<Batch>().Update(batch);
                        }
                    }
                }

                await unitOfWork.Save();



                return new ResponseHandler().Success("Membership request soft deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>($"Error during soft delete: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        public async Task<Response<GetEntityMembershipRequestDto>> GetMembershipRequestById(int requestId)
        {
            try
            {
                var specification = new GeneralSpecifications<MembershipRequest>(
                                   req =>
                                       req.Id == requestId

                               )
                {
                    Includes = new List<Func<IQueryable<MembershipRequest>, IQueryable<MembershipRequest>>>
                        {
                            q => q.Include(r => r.FormUserResponse).ThenInclude(r=>r.FieldsResponse).ThenInclude(r=>r.Field)

                        }
                };

                var request = await unitOfWork.Repository<MembershipRequest>()
                    .GetWithSpecifications(specification);

                // Manually project to DTO
                var membershipDto = new GetEntityMembershipRequestDto
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    FieldsResponse = request.FormUserResponse?.FieldsResponse?.Select(f => new GetRequestFieldResponseDto
                    {
                        Response = f.Response,
                        FieldId = f.FieldId,
                        FieldName = f.Field.Title
                    }).ToList() ?? new List<GetRequestFieldResponseDto>()
                };


                return new ResponseHandler().Success(membershipDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetEntityMembershipRequestDto>(ex.Message);
            }
        }
        public async Task<Response<PaginationToReturnDto<GetAllBatches>>> GetAllBatchesAsync(GetAllBatchesParamDto input)
        {
            var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

            try
            {
                var spec = new GeneralSpecifications<Batch>(
                    b => (!input.Status.HasValue || b.Status == input.Status) && b.TenantId == tenantId || b.TenantId == 0 || b.TenantId == 1)
                {
                    Includes = [b => b.Include(x => x.MembershipRequests)]
                };

                var batches = await unitOfWork.Repository<Batch>().GetAllWithSpecifications(spec);

                if (input.DateFrom.HasValue)
                    if (input.DateFrom.HasValue)
                        batches = batches.Where(b => b.CreationDate >= input.DateFrom.Value).ToList();

                if (input.DateTo.HasValue)
                    batches = batches.Where(b => b.CreationDate <= input.DateTo.Value).ToList();

                if (!string.IsNullOrWhiteSpace(input.SearchTerm))
                {
                    var search = input.SearchTerm.ToLower();
                    batches = batches.Where(b =>
                        b.MembershipRequests.Any(r =>
                            ($"{r.FirstName} {r.MiddleName} {r.LastName}".ToLower().Contains(search)) ||
                            (r.NationalId != null && r.NationalId.ToLower().Contains(search)) ||
                            (r.Email != null && r.Email.ToLower().Contains(search)) ||
                            (r.PhoneNumber != null && r.PhoneNumber.ToLower().Contains(search))
                        )
                    ).ToList();
                }

                var totalCount = batches.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)input.PageSize);

                var pagedBatches = batches
                     .OrderByDescending(b => b.CreationDate)
                    .Skip((input.PageNumber - 1) * input.PageSize)
                    .Take(input.PageSize)
                    .Select(b =>
                    {
                        var mainMember = b.MembershipRequests.FirstOrDefault(r => r.MemberTypeId == 5 || r.IsParent);

                        // Get only children (not parent)
                        var children = b.MembershipRequests.Where(r => !r.IsParent && r.RequestTypeId != 17).ToList();

                        return new GetAllBatches
                        {
                            Id = b.Id,
                            BatchNumber = b.BatchNumber,
                            BatchStatus = b.Status,
                            FullName = mainMember != null
                                ? $"{mainMember.FirstName} {mainMember.MiddleName} {mainMember.LastName}".Trim()
                                : string.Empty,
                            NationalId = mainMember?.NationalId,
                            PhoneNumber = mainMember?.PhoneNumber,
                            Email = mainMember?.Email,
                            DateOfSubmission = b.CreationDate ?? DateTime.MinValue,
                            SubmationPlatform = mainMember?.SubmationPlatform ?? SubmationPlatform.None,
                            TotalAthletes = b.MembershipRequests.Count(r => r.RequestTypeId != 21),
                            PendingAthletes = children.Count(r => r.Status == RequestStatus.Pending),
                            ApprovedAthletes = children.Count(r => r.Status == RequestStatus.RequestResolved),
                            RejectedAthletes = children.Count(r => r.Status == RequestStatus.Rejected)
                        };
                    }).ToList();

                var result = new PaginationToReturnDto<GetAllBatches>(totalCount, totalPages, input.PageSize, input.PageNumber, pagedBatches);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginationToReturnDto<GetAllBatches>>(ex.Message);
            }
        }
        public async Task<Response<PaginationToReturnDto<ChildRequestDto>>> GetAllChildRequestsAsync(GetAllChildRequestParametersDto input)
        {
            var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

            try
            {
                var spec = new GeneralSpecifications<MembershipRequest>(
                    r => !r.IsParent && r.MemberTypeId == 17 && r.TenantId == tenantId)
                {
                    Includes = [b => b.Include(x => x.NewPaymentRequests), b => b.Include(x => x.Batch)]

                };


                var requests = await unitOfWork.Repository<MembershipRequest>().GetAllWithSpecifications(spec);

                if (!string.IsNullOrWhiteSpace(input.SearchTerm))
                {
                    var search = input.SearchTerm.ToLower();
                    requests = requests.Where(r =>
                        ($"{r.FirstName} {r.LastName}".ToLower().Contains(search)) ||
                        (r.NationalId != null && r.NationalId.ToLower().Contains(search)) ||
                        (r.Email != null && r.Email.ToLower().Contains(search)) ||
                        (r.PhoneNumber != null && r.PhoneNumber.ToLower().Contains(search))
                    ).ToList();
                }

                if (input.Gender.HasValue)
                    requests = requests.Where(r => r.Gender == input.Gender).ToList();

                if (!string.IsNullOrWhiteSpace(input.Level))
                    requests = requests.Where(r => r.BeltName != null && r.BeltName.ToLower() == input.Level.ToLower()).ToList();

                if (input.OnlyDraft == false)
                {
                    requests = requests.Where(r => r.Status != RequestStatus.Draft).ToList();
                }
                var totalCount = requests.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)input.PageSize);

                var pagedRequests = requests
                    .Skip((input.PageNumber - 1) * input.PageSize)
                    .Take(input.PageSize)
                    .Select(r => new ChildRequestDto
                    {
                        Id = r.Id,
                        EmailAddress = r.Email,
                        ChildFullName = $"{r.FirstName} {r.MiddleName} {r.LastName}".Trim(),
                        BatchNumber = r.Batch.BatchNumber,
                        NationalId = r.NationalId,
                        DateOfBirth = r.BirthDate ?? DateTime.MinValue,
                        Gender = r.Gender.ToString(),
                        Address = r.Address,
                        PhoneNumber = r.PhoneNumber,
                        Level = r.BeltName ?? "N/A",
                        PackageName = r.Package != null ? r.Package.Name : "N/A",
                        RequestStatus = r.Status,
                        SuperMembershipId = r.SuperMembershipRequestId,
                        //PayemntId = r?.NewPaymentRequests?.OrderBy(r => r.Id).LastOrDefault().PaymentBatchId ?? 0,
                        //PaymentStatus = r?.NewPaymentRequests?.OrderBy(r => r.Id).LastOrDefault().PaymentStatus ?? PaymentStatus.Pending,

                    })
                    .ToList();

                var result = new PaginationToReturnDto<ChildRequestDto>(totalCount, totalPages, input.PageSize, input.PageNumber, pagedRequests);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginationToReturnDto<ChildRequestDto>>(ex.Message);
            }
        }

        public async Task<Response<PaginationToReturnDto<ChildRequestDto>>> GetAllChildRequestsByBatchAsync(
        GetAllChildRequestParametersDto input,
        int batchId)
    {
        var tenantId = int.Parse(
            httpContextAccessor.HttpContext.User.GetTenantId());

        try
        {
            // Get all non-draft child/individual requests in this batch.
            // Parent requests are excluded when IsParent == true.
            var spec = new GeneralSpecifications<MembershipRequest>(
                r => r.TenantId == tenantId
                     && r.BatchId == batchId
                     && r.Status != RequestStatus.Draft
                     && !r.IsParent)
            {
                Includes =
                [
                    b => b.Include(x => x.NewPaymentRequests),
                    b => b.Include(x => x.Package),
                    b => b.Include(x => x.Batch)
                ]
            };

            var requests = await unitOfWork.Repository<MembershipRequest>()
                .GetAllWithSpecifications(spec);

            // Search
            if (!string.IsNullOrWhiteSpace(input.SearchTerm))
            {
                var search = input.SearchTerm.ToLower();

                requests = requests
                    .Where(r =>
                        ($"{r.FirstName} {r.LastName}"
                            .ToLower()
                            .Contains(search))
                        ||
                        (r.NationalId != null &&
                         r.NationalId.ToLower().Contains(search))
                        ||
                        (r.Email != null &&
                         r.Email.ToLower().Contains(search))
                        ||
                        (r.PhoneNumber != null &&
                         r.PhoneNumber.ToLower().Contains(search))
                    )
                    .ToList();
            }

            // Gender
            if (input.Gender.HasValue)
            {
                requests = requests
                    .Where(r => r.Gender == input.Gender)
                    .ToList();
            }

            // Level
            if (!string.IsNullOrWhiteSpace(input.Level))
            {
                requests = requests
                    .Where(r =>
                        r.BeltName != null &&
                        r.BeltName.ToLower() == input.Level.ToLower())
                    .ToList();
            }

            var totalCount = requests.Count;

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)input.PageSize);

            var pagedRequests = requests
                .OrderByDescending(r => r.Id)
                .Skip((input.PageNumber - 1) * input.PageSize)
                .Take(input.PageSize)
                .Select(r => new ChildRequestDto
                {
                    Id = r.Id,

                    BatchNumber = r.Batch != null
                        ? r.Batch.BatchNumber
                        : null,

                    EmailAddress = r.Email,

                    ChildFullName =
                        $"{r.FirstName} {r.MiddleName} {r.LastName}".Trim(),

                    NationalId = r.NationalId,

                    DateOfBirth =
                        r.BirthDate ?? DateTime.MinValue,

                    Gender =
                        r.Gender.ToString(),

                    Address = r.Address,

                    PhoneNumber = r.PhoneNumber,

                    Level =
                        r.BeltName ?? "N/A",

                    PackageName =
                        r.Package != null
                            ? r.Package.Name
                            : "N/A",

                    RequestStatus = r.Status,

                    // Parent ID
                    // For children, this points to the parent request.
                    SuperMembershipId =
                        r.SuperMembershipRequestId,

                    IsParent = r.IsParent,

                    MemberTypeId = r.MemberTypeId,

                    IsIndividual =
                        !r.IsParent &&
                        r.SuperMembershipRequestId == null,

                    // Payment
                    PayemntId = r.NewPaymentRequests?
                        .OrderBy(x => x.Id)
                        .LastOrDefault()
                        ?.PaymentBatchId ?? 0,

                    PaymentStatus = r.NewPaymentRequests?
                        .OrderBy(x => x.Id)
                        .LastOrDefault()
                        ?.PaymentStatus
                        ?? PaymentStatus.Pending
                })
                .ToList();

            var result = new PaginationToReturnDto<ChildRequestDto>(
                totalCount,
                totalPages,
                input.PageSize,
                input.PageNumber,
                pagedRequests);

            return new ResponseHandler().Success(result);
        }
        catch (Exception ex)
        {
            return new ResponseHandler()
                .BadRequest<PaginationToReturnDto<ChildRequestDto>>(
                    ex.InnerException?.Message ?? ex.Message);
        }
    }
        public async Task<Response<CurrentMembershipDto>> ApproveRequestAndCreateMembershipWithDates(ApproveMembershipRequestDto approveMembershipRequestDto)
        {
            try
            {

                var chashKey = $"GetCurrentUserMembershipRequestsWithChildren:{approveMembershipRequestDto.RequestId}";
                if (memoryCache.TryGetValue(chashKey, out CurrentMembershipDto cachedDto))
                {
                    memoryCache.Remove(chashKey);
                }
                // Step 1: Get the request

                var specification = new GeneralSpecifications<MembershipRequest>(r => r.Id == approveMembershipRequestDto.RequestId)
                {
                    Includes = new List<Func<IQueryable<MembershipRequest>, IQueryable<MembershipRequest>>>
                    {
                        q => q.Include(r => r.FormUserResponse)
                        .ThenInclude(fur => fur.FieldsResponse)
                    }
                };

                var request = await unitOfWork.Repository<MembershipRequest>()
                    .GetWithSpecifications(specification);

                if (request == null)
                    return new ResponseHandler().NotFound<CurrentMembershipDto>("Membership request not found.");

                if (request.Status != RequestStatus.Pending)
                    return new ResponseHandler().BadRequest<CurrentMembershipDto>("Request is not pending.");
                if (request.Status != RequestStatus.Pending)
                    return new ResponseHandler().BadRequest<CurrentMembershipDto>("Request already approved or not pending.");

                if (request.PackageId == null)
                    return new ResponseHandler().BadRequest<CurrentMembershipDto>("No package linked to this request.");

                var package = await unitOfWork.Repository<Package>().GetByIdAsync(request.PackageId.Value);
                if (package == null)
                    return new ResponseHandler().BadRequest<CurrentMembershipDto>("Invalid package.");

                // Step 4: Validate duration
                int allowedMonths = package.Duration ?? 0;
                int actualMonths = ((approveMembershipRequestDto.EndDate.Year - approveMembershipRequestDto.StartDate.Year) * 12) + approveMembershipRequestDto.EndDate.Month - approveMembershipRequestDto.StartDate.Month;

                if (actualMonths > allowedMonths)
                {
                    return new ResponseHandler().BadRequest<CurrentMembershipDto>(
                        $"Membership duration exceeds package duration. Allowed: {allowedMonths} months, Provided: {actualMonths} months.");
                }

                request.Status = RequestStatus.RequestResolved;

                var membership = mapper.Map<Data.Membership.Entities.Membership>(request);

                membership.StartDate = approveMembershipRequestDto.StartDate;
                membership.EndDate = approveMembershipRequestDto.EndDate;
                membership.MemberShipId = GenerateMembershipId();
                membership.MembershipStatus = MembershipStatus.Active;

                await unitOfWork.Repository<Data.Membership.Entities.Membership>().Add(membership);
                await unitOfWork.Save();
                await ChangeMembershipStatus(membership.Id, MembershipStatus.Active);

                request.MembershipId = membership.Id;

                unitOfWork.Repository<MembershipRequest>().Update(request);


                try
                {
                    await ScheduleMembershipExpireJob(membership.Id, request.Id);
                }
                catch (Exception ex)
                {
                    Log.Information($"No existing membership found in target tenant. Creating a new one{membership.Id}:{ex}");
                    return new ResponseHandler().BadRequest<CurrentMembershipDto>(ex.InnerException?.Message ?? ex.Message);
                }

                if (membership.PackageId.HasValue)
                {
                    var branchFacilitiesVenues = await unitOfWork
                        .Repository<MembershipRequestAccess>()
                        .GetAllAsync(fv => fv.MembershipRequestId == request.Id);

                    foreach (var fv in branchFacilitiesVenues)
                    {
                        var access = new MembershipAccess
                        {
                            MembershipId = membership.Id,
                            BranchId = fv.BranchId,
                            FacilityId = fv.FacilityId,
                            VenueId = fv.VenueId
                        };

                        await unitOfWork.Repository<MembershipAccess>().Add(access);
                    }
                }
                await unitOfWork.Save();


                string? levelOption = request.FormUserResponse?.FieldsResponse?
                    .FirstOrDefault(fr => fr.FieldId == 97)?.Response;

                if (!string.IsNullOrEmpty(levelOption))
                {
                    await _publishEndpoint.Publish(new UserLevelUpdatedEvent
                    {
                        UserId = membership.UserId,
                        TenantId = membership.TenantId,
                        OptionName = levelOption,
                        ChangedBy = _httpContextAccessor.HttpContext?.User.GetUserId()
                    });

                    Log.Information($"Published UserLevelUpdatedEvent for UserId={membership.UserId}, Level={levelOption}");
                }

                await _publishEndpoint.Publish(new UserTeanatAccessCreatedDto
                {
                    UserId = membership.UserId,
                    TenantId = membership.TenantId
                });

                var dto = new CurrentMembershipDto
                {
                    Id = membership.Id,
                    MembershipStatus = membership.MembershipStatus,
                };

                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<CurrentMembershipDto>(
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }
        public async Task<Response<string>> SetBatchRequestsToPending(int batchId)
        {
            try
            {
                // Step 1: Get the batch including its requests
                var batch = await unitOfWork.Repository<Batch>()
                    .GetByIdAsync(batchId);

                if (batch == null)
                    return new ResponseHandler().NotFound<string>("Batch not found.");

                var requests = await unitOfWork.Repository<MembershipRequest>()
                    .FindAllByCriteriaAsync(r => r.BatchId == batchId);

                if (!requests.Any())
                    return new ResponseHandler().NotFound<string>("No membership requests found for this batch.");

                var type11Requests = requests.Where(r => r.RequestTypeId == 21).ToList();
                var otherRequests = requests.Where(r => r.RequestTypeId != 21).ToList();

                var invalidRequests = otherRequests.Where(r => r.PackageId == null).ToList();
                if (invalidRequests.Any())
                {
                    var names = invalidRequests.Select(r =>
                        $"{r.FirstName} {r.MiddleName} {r.LastName}".Trim()).ToList();

                    string invalidUsers = string.Join(", ", names);

                    return new ResponseHandler().BadRequest<string>(
                        $"Some requests have no package assigned: {invalidUsers}"
                    );
                }

                // Step 4: Update all requests to Pending
                foreach (var req in requests)
                {
                    req.Status = RequestStatus.Pending;
                    unitOfWork.Repository<MembershipRequest>().Update(req);
                }

                batch.Status = BatchStatus.Submited;
                unitOfWork.Repository<Batch>().Update(batch);

                await unitOfWork.Save();

                return new ResponseHandler().Success("Batch and all requests updated to Pending.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>(
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        private static string GenerateMembershipId()
        {
            Guid guid = Guid.NewGuid();
            int uniqueNumber = Math.Abs(BitConverter.ToInt32(guid.ToByteArray(), 0));
            string timestamp = DateTime.Now.Ticks.ToString();
            return uniqueNumber.ToString() + timestamp;
        }

        private async Task<Response<Data.Membership.Entities.Membership>> ScheduleMembershipExpireJob(int membershipId, int membershipRequestId)
        {
            try
            {
                var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .GetByIdAsync(membershipId);

                if (membership == null)
                    return new ResponseHandler().NotFound<Data.Membership.Entities.Membership>("Membership not found.");

                if (!membership.EndDate.HasValue)
                    return new ResponseHandler().BadRequest<Data.Membership.Entities.Membership>("Membership has no EndDate.");

                // Calculate delay from now to end date
                var delay = membership.EndDate.Value - DateTime.UtcNow;
                if (delay < TimeSpan.Zero)
                    delay = TimeSpan.Zero; // already expired → schedule immediately

                // Create background job entry
                var expireJob = new MembershipRequestBackgroundJobs
                {
                    MembershipRequestId = membershipRequestId,
                };

                expireJob.BackgroundJobId = backgroundJobClient.Schedule(
                    () => ChangeMembershipStatus(membership.Id, MembershipStatus.NotActive),
                    delay
                );

                await unitOfWork.Repository<MembershipRequestBackgroundJobs>().Add(expireJob);

                return new ResponseHandler().Success(membership);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<Data.Membership.Entities.Membership>(ex.Message);
            }
        }
        public async Task ChangeMembershipStatus(int membershipId, MembershipStatus status)
        {
            var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(membershipId);
            membership.MembershipStatus = status;
            unitOfWork.Repository<Data.Membership.Entities.Membership>().Update(membership);
            await _publishEndpoint.Publish(new UpdateMembershipStatus
            {
                UserId = membership.UserId,
                Status = membership.MembershipStatus != MembershipStatus.Active ? false : true,
                TenantId = membership.TenantId

            });
            if (status == MembershipStatus.Active)
            {
                await _publishEndpoint.Publish(new UserTeanatAccessCreatedDto
                {
                    UserId = membership.UserId,
                    TenantId = membership.TenantId

                });
            }
            else
            {
                await _publishEndpoint.Publish(new UserTenantAccessRemovedDto
                {
                    UserId = membership.UserId,
                    TenantId = membership.TenantId

                });
            }
            await unitOfWork.Save();
        }


        private void SetUserData(DTOs.Membership.UserDtos.UserDataDto userData, MembershipRequest request)
        {
            userData.TenantData.EntityId = request.TenantId;
            userData.TenantData.EntityName = request.Tenant.Name;
            userData.TenantData.SuperEntityId = request.SuperTenantId ?? 0;
            userData.TenantData.SuperEntityName = request?.SuperTenant?.Name!;

        }

    }
}

