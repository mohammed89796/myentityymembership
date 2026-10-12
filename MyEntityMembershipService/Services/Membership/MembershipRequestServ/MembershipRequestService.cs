using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.NewMembershipDtos;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;
using MyEntityMembershipService.DTOs.Membership.TenantDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.FormUserResponseRepo;
using MyEntityMembershipService.Repositories.Membership.MembershipRepo;
using MyEntityMembershipService.Repositories.Membership.MembershipRequestRepository;
using MyEntityMembershipService.Repositories.Membership.PaakageMemberFormRepo;
using MyEntityMembershipService.Repositories.Membership.PackageRequestAuthourizationsRepo;
using MyEntityMembershipService.Repositories.Membership.RequestTypeRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.Services.Membership.FormServ;
using MyEntityMembershipService.Services.Membership.FreezeConfigurationService;
using MyEntityMembershipService.SpecificationPattern;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.MembershipRequestServ
{
    public class MembershipRequestService : IMembershipRequestService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IMembershipRequestRepository membershipRequestRepository;
        private readonly IRequestTypeRepository requestTypeRepository;
        private readonly IMembershipRepository membershipRepository;
        private readonly IPackageRequestAuthorizationRepository packageRequestAuthorizationRepository;
        private readonly IFormService formService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IFormUserResponseRepository formUserResponseRepository;
        private readonly IFreezeConfigurationService freezeConfigurationService;
        private readonly IPaakageMemberFormRepo paakageMemberForm;
        private readonly IFetchService fetchService;
        private readonly IMemoryCache memoryCache;

        public MembershipRequestService(IUnitOfWork unitOfWork, IMapper mapper,
                                        IConfiguration configuration, IMembershipRequestRepository membershipRequestRepository,
                                         IRequestTypeRepository requestTypeRepository, IMembershipRepository membershipRepository,
                                         IPackageRequestAuthorizationRepository packageRequestAuthorizationRepository, IFormService formService,
                                         IHttpContextAccessor httpContextAccessor, IFormUserResponseRepository formUserResponseRepository,
                                         IFreezeConfigurationService freezeConfigurationService,
                                         IPaakageMemberFormRepo paakageMemberForm,
                                         IFetchService fetchService, IMemoryCache memoryCache)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.configuration = configuration;
            this.membershipRequestRepository = membershipRequestRepository;
            this.requestTypeRepository = requestTypeRepository;
            this.membershipRepository = membershipRepository;
            this.packageRequestAuthorizationRepository = packageRequestAuthorizationRepository;
            this.formService = formService;
            this.httpContextAccessor = httpContextAccessor;
            this.formUserResponseRepository = formUserResponseRepository;
            this.freezeConfigurationService = freezeConfigurationService;
            this.paakageMemberForm = paakageMemberForm;
            this.fetchService = fetchService;
            this.memoryCache = memoryCache;
        }

        public async Task<Response<AddMembershipRequestDto>> AddMembershipRequest(AddMembershipRequestDto RequestDto)
        {
            try
            {
                var existingMembership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(RequestDto.MembershipId);
                if (existingMembership is not null)
                {
                    var request = mapper.Map<MembershipRequest>(RequestDto);
                    var ExistingMembershipRequestType = await requestTypeRepository.GetTypeByName("New Membership Request");
                    request.RequestTypeId = ExistingMembershipRequestType.Id;
                    request.FirstName = existingMembership.FirstName;
                    request.BirthDate = existingMembership.BirthDate;
                    request.UserPhoto = existingMembership.UserPhoto;
                    await unitOfWork.Repository<MembershipRequest>().Add(request);
                    await unitOfWork.Save();
                    return new ResponseHandler().Success(RequestDto);
                }
                return new ResponseHandler().BadRequest<AddMembershipRequestDto>("There is no Membership With this ID");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AddMembershipRequestDto>(ex.Message);
            }
        }
        public async Task<Response<ReturnMembershipRequestDto>> CreateMembershipRequest(CreateMembershipRequestDto RequestDto)
        {
            try
            {
                var request = mapper.Map<MembershipRequest>(RequestDto);
                request.NationalIdPhoto = RequestDto.NationalIdImage is not null ? await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", RequestDto.NationalIdImage) : "";
                request.UserPhoto = RequestDto.UserImage is not null ? await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", RequestDto.UserImage) : "";
                var createRequestType = await requestTypeRepository.GetTypeByName("New Membership Request");
                request.RequestTypeId = createRequestType.Id;
                request.Status = RequestStatus.Draft;
                await unitOfWork.Repository<MembershipRequest>().Add(request);
                await unitOfWork.Save();
                var result = mapper.Map<ReturnMembershipRequestDto>(request);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<ReturnMembershipRequestDto>(ex.Message);
            }
        }
        public async Task<Response<SubMemberDto>> AddSubMember(SubMemberDto SubMemberDto)
        {
            try
            {
                var request = await unitOfWork.Repository<MembershipRequest>().GetByIdAsync(SubMemberDto.SuperMembershipId.Value);
                if (request is null)
                    return new ResponseHandler().BadRequest<SubMemberDto>("There is No Membership Request with that Id");
                var subMember = mapper.Map<MembershipRequest>(SubMemberDto);
                subMember.SuperMembershipRequestId = request.Id;
                subMember.NationalIdPhoto = SubMemberDto.NationalIdImage is not null ? await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", SubMemberDto.NationalIdImage) : "";
                subMember.UserPhoto = SubMemberDto.UserImage is not null ? await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", SubMemberDto.UserImage) : "";
                subMember.TenantId = request.TenantId;
                subMember.IsTermsAgreed = request.IsTermsAgreed;
                subMember.PackageId = request.PackageId;
                subMember.Address = request.Address;
                subMember.Country = request.Country;
                subMember.City = request.City;
                subMember.Governate = request.Governate;
                subMember.Email = request.Email;
                subMember.PhoneNumber = request.PhoneNumber;
                subMember.UserId = request.UserId;
                subMember.Status = RequestStatus.Draft;
                subMember.RequestTypeId = request.RequestTypeId;
                await unitOfWork.Repository<MembershipRequest>().Add(subMember);
                await unitOfWork.Save();
                SubMemberDto = mapper.Map<SubMemberDto>(subMember);
                return new ResponseHandler().Success(SubMemberDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<SubMemberDto>(ex.Message);
            }
        }
        public async Task<Response<AddMembershipWaiverRequestDto>> CreateMembershipWaiverRequest(AddMembershipWaiverRequestDto membershipWaiverRequestDto)
        {
            try
            {
                var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(membershipWaiverRequestDto.MembershipId);
                if (membership is null)
                    return new ResponseHandler().NotFound<AddMembershipWaiverRequestDto>("There is no Membership with that Id");
                var request = mapper.Map<MembershipRequest>(membershipWaiverRequestDto);
                var waiverRequestType = await requestTypeRepository.GetTypeByName("Membership Waiver");
                request.RequestTypeId = waiverRequestType.Id;
                await unitOfWork.Repository<MembershipRequest>().Add(request);
                await unitOfWork.Save();
                membershipWaiverRequestDto = mapper.Map<AddMembershipWaiverRequestDto>(request);
                return new ResponseHandler().Success(membershipWaiverRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AddMembershipWaiverRequestDto>(ex.Message);
            }
        }

        public async Task<Response<AddMembershipSeparationRequestDto>> CreateMembershipSeparationRequest(AddMembershipSeparationRequestDto separationRequestDto)
        {
            try
            {
                var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(separationRequestDto.MembershipId);
                if (membership is null)
                    return new ResponseHandler().NotFound<AddMembershipSeparationRequestDto>("There is no Membership with that Id");
                var request = mapper.Map<MembershipRequest>(separationRequestDto);
                var separationRequestType = await requestTypeRepository.GetTypeByName("Separation Request");
                request.RequestTypeId = separationRequestType.Id;
                await unitOfWork.Repository<MembershipRequest>().Add(request);
                await unitOfWork.Save();
                separationRequestDto = mapper.Map<AddMembershipSeparationRequestDto>(request);
                return new ResponseHandler().Success(separationRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AddMembershipSeparationRequestDto>(ex.Message);
            }
        }

        public async Task<Response<AddMembershipModificationRequestDto>> CreateMembershipModificationRequest(AddMembershipModificationRequestDto modificationRequestDto)
        {
            try
            {
                var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(modificationRequestDto.MembershipId);
                if (membership is null)
                    return new ResponseHandler().NotFound<AddMembershipModificationRequestDto>("There is no Membership with that Id");
                var request = mapper.Map<MembershipRequest>(modificationRequestDto);
                var modificationRequestType = await requestTypeRepository.GetTypeByName("Membership Modification");
                request.RequestTypeId = modificationRequestType.Id;
                await unitOfWork.Repository<MembershipRequest>().Add(request);
                await unitOfWork.Save();
                modificationRequestDto = mapper.Map<AddMembershipModificationRequestDto>(request);
                return new ResponseHandler().Success(modificationRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AddMembershipModificationRequestDto>(ex.Message);
            }
        }

        public async Task<Response<List<CurrentMembershipDto>>> GetCurrentUserMemberships(string userId, string name)
        {
            try
            {
                var userMemberships = string.IsNullOrEmpty(name) ?
                    await membershipRepository.GetUserMemberships(userId)
                    : await membershipRepository.GetUserMembershipsByName(userId, name);
                var result = mapper.Map<List<CurrentMembershipDto>>(userMemberships);
                foreach (var membership in userMemberships)
                {
                    var resultItem = result.Find(item => item.Id == membership.Id);

                    var client = new HttpClient();
                    var url = $"{configuration["ApiSettings:Gateway"]}/EMembership/1/Package/GetPackageByIdAsync/{membership.PackageId}";
                    var token = httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetStringAsync(url);
                    var package = JsonConvert.DeserializeObject<Response<PackageDataDto>>(response);
                    resultItem.TenantName = package.Data.Tenant.Name;
                    resultItem.PackageName = package.Data.Name;
                    resultItem.TenantImage = package.Data.Tenant.Image;
                    //var tenant = await unitOfWork.Repository<Tenant>().GetByIdAsync(membership.TenantId);
                    //resultItem.TenantName = tenant.Name;
                    var authorizations = await GetAuthourizationDataByPackage(membership.PackageId.Value, resultItem);
                    resultItem.MembershipAuthorizations = authorizations.Data.MembershipAuthorizations;
                    var freezConfig = await freezeConfigurationService.GetByPackageId(membership.PackageId.Value);
                    if (freezConfig.Data is null)
                    {
                        var freezAuth = resultItem.MembershipAuthorizations.FirstOrDefault(auth => auth.Id == 12);
                        if (freezAuth is not null)
                            resultItem.MembershipAuthorizations.Remove(freezAuth);
                    }
                }
                var tasks = result.Select(async dto =>
                {
                    dto.TenantImage = await DocumentSetting.GetPreSignedUrlAsync(dto.TenantImage);

                    dto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.UserPhoto);



                }).ToList();

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<CurrentMembershipDto>>(ex.Message);
            }
        }

        public async Task<Response<CurrentMembershipDto>> GetAuthourizationDataByPackage(int packageId, CurrentMembershipDto resultItem)
        {
            try
            {
                var authorizations = await packageRequestAuthorizationRepository.GetPackageAuthorizations(packageId);
                resultItem.MembershipAuthorizations = mapper.Map<List<MembershipAuthorizationDto>>(authorizations);
                // used for loop 
                List<MembershipAuthorizationDto> membershipAuthorizations = new List<MembershipAuthorizationDto>();
                membershipAuthorizations.AddRange(resultItem.MembershipAuthorizations);
                // get All forms Per Package
                var packageForms = await packageRequestAuthorizationRepository.GetPaakageMemberForms(packageId);
                // get All Package Forms Grouped Request Type
                var requestTypesForms = packageForms.GroupBy(item => item.Form.RequestTypeId);
                foreach (var item in membershipAuthorizations)
                {
                    // get All Package Forms for each Request Type exist in MembershipAuthorizations
                    var requestTypeMembers = requestTypesForms.FirstOrDefault(type => type.Key == item.Id);
                    if (requestTypeMembers is null)
                        resultItem.MembershipAuthorizations.Remove(item);
                    else
                    {
                        var memberTypeForms = requestTypeMembers.GroupBy(mtf => mtf.MemberTypeId).ToList();
                        var authorizationMemberTypesDto = new List<AuthorizationMemberTypesDto>();
                        foreach (var memberType in memberTypeForms)
                        {
                            var memberTypeDtata = await unitOfWork.Repository<MemberType>().GetByIdAsync(memberType.Key);
                            var authorizationMemberTypeDto = new AuthorizationMemberTypesDto()
                            {
                                Id = memberType.Key,
                                FormId = memberType.First().FormId,
                                Name = memberTypeDtata.Name
                            };
                            authorizationMemberTypesDto.Add(authorizationMemberTypeDto);
                        }
                        resultItem.MembershipAuthorizations.Find(ma => ma.Id == item.Id).MemberTypes = authorizationMemberTypesDto;
                    }

                }
                return new ResponseHandler().Success(resultItem);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<CurrentMembershipDto>(ex.Message);
            }
        }

        public async Task<Response<List<CurrentMembershipRequestDto>>> GetCurrentUserMembershipRequest(string userId, string name)
        {
            try
            {
                var requests = string.IsNullOrEmpty(name) ?
                    await membershipRequestRepository.GetUserMembershipRequests(userId)
                    : await membershipRequestRepository.GetMembershipRequestsByName(userId, name);
                var result = mapper.Map<List<CurrentMembershipRequestDto>>(requests);
                foreach (var request in requests)
                {
                    var type = await unitOfWork.Repository<RequestType>().GetByIdAsync(request.RequestTypeId.Value);
                    result.Find(item => item.Id == request.Id).RequestType = type.Name;
                }

                var tasks = result.Select(async dto =>
                {


                    dto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.UserPhoto);


                }).ToList();

                await Task.WhenAll(tasks);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<CurrentMembershipRequestDto>>(ex.Message);
            }
        }

        public async Task<Response<GetMembershipRequestDto>> GetMembershipRequest(int Id)
        {
            try
            {
                // Get Membership Requests 
                var membershipRequest = await membershipRequestRepository.GetMembershipRequestWithStatusHistory(Id);
                if (membershipRequest is null)
                    return new ResponseHandler().BadRequest<GetMembershipRequestDto>("There is no Membership with that Id");
                var membershipRequestDto = mapper.Map<GetMembershipRequestDto>(membershipRequest);
                // Mapping SubMembers
                if (membershipRequest.SubMembers is not null)
                    membershipRequestDto.SubMembers = mapper.Map<List<SubMemberDataDto>>(membershipRequest.SubMembers);
                // Get Member Type Name
                var membershipMemberType = await unitOfWork.Repository<MemberType>().GetByIdAsync(membershipRequest.MemberTypeId.Value);
                membershipRequestDto.MemberTypeName = membershipMemberType.Name;
                // Get Main Member Response
                var formUserResponse = await formUserResponseRepository.GetFormUserResponseWithForm(membershipRequest.Id);
                membershipRequestDto.FormUserResponseDto = mapper.Map<FormUserResponseDto>(formUserResponse);
                var documents = formUserResponse.FieldsResponse.Where(response => IsFileField(response.Field.FieldTypeId)).ToList();

                // To Get Updated Form Fees from Configuration
                var packageMemberForm = new PaakageMemberForm();
                if (membershipRequestDto.RequestTypeId == 9 || membershipRequestDto.RequestTypeId == 18 || membershipRequestDto.RequestTypeId == 16)
                {
                    membershipRequestDto.SubMembers[0].FormUserResponseDto = membershipRequestDto.FormUserResponseDto;
                    membershipRequestDto.FormUserResponseDto = null;
                    membershipRequestDto.SubMembers[0].DocumentsSubmitted = mapper.Map<List<DocumentSubmitted>>(documents);

                    // To Get Form Fees updated from Configuration According to SubMember Member Type
                    packageMemberForm = await paakageMemberForm.GetMemberFormByFormIdPackageIdmemberTypeId(membershipRequest.PackageId.Value, formUserResponse.FormId, membershipRequestDto.SubMembers[0].SubMemberDto.MemberTypeId.Value);

                }
                else
                {
                    membershipRequestDto.DocumentsSubmitted = mapper.Map<List<DocumentSubmitted>>(documents);
                    packageMemberForm = await paakageMemberForm.GetMemberFormByFormIdPackageIdmemberTypeId(membershipRequest.PackageId.Value, formUserResponse.FormId, membershipRequest.MemberTypeId.Value);

                }
                if(packageMemberForm !=null && packageMemberForm.Fees !=null)
                {
                    membershipRequestDto.FormFees = packageMemberForm.Fees;

                }
                if (membershipRequest.RequestStatusHistories.Count > 0)
                    membershipRequestDto.LastStatusUpdate = membershipRequest.RequestStatusHistories.Last().CreationDate;

                membershipRequestDto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipRequestDto.NationalIdPhoto);

                membershipRequestDto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipRequestDto.UserPhoto);

                return new ResponseHandler().Success(membershipRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetMembershipRequestDto>(ex.Message);
            }
        }
        private bool IsFileField(int fieldTypeId) => fieldTypeId == 6;

        public async Task<Response<GetMembershipDto>> GetMembership(int Id)
        {
            try
            {
                var membership = await membershipRepository.GetMembershipWithSubMembers(Id);
                if (membership is null)
                    return new ResponseHandler().BadRequest<GetMembershipDto>("there is no membership with that Id");
                var membershipDto = new GetMembershipDto()
                {
                    Id = membership.Id,
                    Status = (int)membership.MembershipStatus,
                    Address = membership.Address,
                    Email = membership.Email,
                    Country = membership.Country,
                    City = membership.City,
                    BirthDate = membership.BirthDate,
                    Gender = membership.Gender,
                    Governate = membership.Governate,
                    MembershipId = membership.MemberShipId,
                    MemberSince = membership.StartDate,
                    FirstName = membership.FirstName,
                    LastName = membership.LastName,
                    NationalId = membership.NationalId,
                    Nationality = membership.Nationality,
                    NationalIdPhoto = membership.NationalIDPhoto,
                    PhoneNumber = membership.PhoneNumber,
                    TenantId = membership.TenantId,
                    UserPhoto = membership.UserPhoto,
                    PackageId = membership.PackageId,
                    MemberTypeId = membership.MemberTypeId,
                    ExpiryDate = membership.EndDate
                };
                var membershipMemberType = await unitOfWork.Repository<MemberType>().GetByIdAsync(membershipDto.MemberTypeId.Value);
                membershipDto.MemberTypeName = membershipMemberType.Name;
                membershipDto.SubMembers = membership.SubMembers is not null ? mapper.Map<List<SubMemberDto>>(membership.SubMembers) : null;
                foreach (var submember in membershipDto.SubMembers)
                {
                    var submemberMemberType = await unitOfWork.Repository<MemberType>().GetByIdAsync(submember.MemberTypeId.Value);
                    submember.MemberTypeName = submemberMemberType.Name;
                    var subMembership = membership.SubMembers.FirstOrDefault(sub => sub.Id == submember.Id);
                    submember.MemberSince = subMembership?.EndDate;
                    submember.ExpiryDate = subMembership?.StartDate;
                }
                var client = new HttpClient();
                var url = $"{configuration["ApiSettings:Gateway"]}/EMembership/1/Tenant/GetTenantById/{membershipDto.TenantId}";
                var token = httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetStringAsync(url);
                var tenant = JsonConvert.DeserializeObject<Response<TenantReturnDto>>(response);
                membershipDto.TenantName = tenant.Data.Name;

                membershipDto.NationalIdPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.NationalIdPhoto);
                membershipDto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.UserPhoto);

                return new ResponseHandler().Success(membershipDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetMembershipDto>(ex.Message);
            }
        }

        public async Task<Response<List<ActivityDto>>> GetMembershipRequestActivity(int Id)
        {
            try
            {
                var membershipRequestComments = await membershipRequestRepository.GetMembershipRequestComments(Id);
                var activity = mapper.Map<List<ActivityDto>>(membershipRequestComments);
                var membershipRequestPayments = await membershipRequestRepository.GetMembershipRequestPayments(Id);
                var paymentComments = mapper.Map<List<ActivityDto>>(membershipRequestPayments);
                activity.AddRange(paymentComments);
                activity = activity.OrderByDescending(activity => activity.CreationDate).ToList();
                return new ResponseHandler().Success(activity);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<ActivityDto>>(ex.Message);
            }
        }

        public async Task<Response<GetMembershipRequestDto>> SetPendingStatus(int Id)
        {
            try
            {
                var request = await unitOfWork.Repository<MembershipRequest>().GetByIdAsync(Id);
                request.Status = RequestStatus.Pending;
                await unitOfWork.Save();
                var membershipRequestDto = mapper.Map<GetMembershipRequestDto>(request);
                var submembers = await membershipRequestRepository.GetMembershipSubmembers(Id);
                foreach (var submember in submembers)
                    submember.Status = RequestStatus.Pending;
                await unitOfWork.Save();
                //if (submembers is not null)
                //    membershipRequestDto.SubMembers = mapper.Map<List<SubMemberDto>>(submembers);
                return new ResponseHandler().Success(membershipRequestDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetMembershipRequestDto>(ex.Message);
            }
        }

        public async Task<Response<PaginatedFedEntityRequestsList>> GetFedEntityRequests(int tenantID, int typeId, FedEntityRequestFilterDto model)
        {
            try
            {
                var result = await membershipRequestRepository.GetFedEntityRequestsByTenantId(tenantID, typeId, model);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginatedFedEntityRequestsList>("Something went wrong");
            }
        }
        public async Task<Response<PaginatedFedRequestsList>> GetFedRequests(int tenantID, int typeId, FedRequestFilterDto model)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();
                var admin = await fetchService.GetTenantAdmin(userId);
                var areaIds = admin.TenantAreaAdmins.Select(area => area.AreaId).ToList();
                var result = await membershipRequestRepository.GetFedRequestsByTenantId(tenantID, areaIds, typeId, model);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginatedFedRequestsList>("Something went wrong");
            }
        }
        public async Task<Response<PaginatedFedEntityRequestsList>> GetFedEntityRequestsByBatchID(int batchID, int typeId, FedEntityRequestByBatchFilterDto model)
        {
            try
            {
                var result = await membershipRequestRepository.GetFedEntityRequestsByBatchId(batchID, typeId, model);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginatedFedEntityRequestsList>("Something went wrong");
            }
        }
        public async Task<Response<PaginatedFederationEntityBatchList>> GetFedEntityBatchList(int tenantID, int typeId, FedEntityBatchFilterDto filter)
        {
            try
            {
                var result = await membershipRequestRepository.GetFedEntityBatchesByTenantId(tenantID, typeId, filter);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginatedFederationEntityBatchList>("Something went wrong");
            }
        }
        public async Task<Response<PaginatedFederationEntityBatchList>> GetFedEntityDraftBatchList(int tenantID, int typeId, FedEntityBatchFilterDto filter)
        {
            try
            {
                var result = await membershipRequestRepository.GetFedEntityDraftBatchesByTenantId(tenantID, typeId, filter);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginatedFederationEntityBatchList>("Something went wrong");
            }
        }
        public async Task<Response<PaginatedFederationBatchList>> GetFedBatchList(int tenantID, int typeId, FedBatchFilterDto filter)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();
                var admin = await fetchService.GetTenantAdmin(userId);
                var areaIds = admin.TenantAreaAdmins.Select(area => area.AreaId).ToList();
                var result = await membershipRequestRepository.GetFedBatchesByTenantId(tenantID, areaIds, typeId, filter);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginatedFederationBatchList>("Something went wrong");
            }
        }
        public async Task<Response<PaginatedFederationBatchList>> GetFedBatchesByTenantId(int tenantID, int typeId, FedBatchFilterDto filter)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();
                var admin = await fetchService.GetTenantAdmin(userId);
                var areaIds = admin.TenantAreaAdmins.Select(area => area.AreaId).ToList();
                var result = await membershipRequestRepository.GetFedBatchesByTenantId(tenantID, areaIds, typeId, filter);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginatedFederationBatchList>("Something went wrong");
            }
        }
        public async Task<Response<MembershipRequestDetailsDto>> GetRequestDetails(int requestId)
        {
            try
            {
                var result = await membershipRequestRepository.GetMembershipRequestDetails(requestId);

                if (result is not null)
                    result.ProfileImageURL = await DocumentSetting.GetPreSignedUrlAsync(
                result.ProfileImageURL);


                return new ResponseHandler().Success<MembershipRequestDetailsDto>(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<List<MembershipRequestDetailsByBatchDto>>> GetRequestDetailsByBatchID(int batchId)
        {
            try
            {
                var result = await membershipRequestRepository.GetMembershipRequestsByBatch(batchId);

                var tasks = result.Select(async dto =>
                {
                    dto.ProfileImageURL = await DocumentSetting.GetPreSignedUrlAsync(
                        dto.ProfileImageURL);
                }).ToList();
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MembershipRequestDetailsByBatchDto>>("Something went wrong");
            }
        }
        public async Task<Response<List<MembershipRequestDetailsDto>>> GetBatchDetailedView(int batchId)
        {
            try
            {
                var result = await membershipRequestRepository.GetBatchDetailedViewAsync(batchId);

                var tasks = result.Select(async dto =>
                {
                    dto.ProfileImageURL = await DocumentSetting.GetPreSignedUrlAsync(
                        dto.ProfileImageURL);
                }).ToList();

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MembershipRequestDetailsDto>>("Something went wrong");
            }
        }
        public async Task<Response<GetFedEntityAnalytics>> GetFedEntityMembershipAnalytics(int tenantID, int typeId)
        {
            try
            {
                var result = await membershipRequestRepository.GetFedEntityMembershipAnalyticsAsync(tenantID, typeId);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetFedEntityAnalytics>("Something went wrong");
            }
        }
        public async Task<Response<GetFedEntityAnalytics>> GetFedMembershipAnalytics(int tenantID, int typeId)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();
                var admin = await fetchService.GetTenantAdmin(userId);
                var areaIds = admin.TenantAreaAdmins.Select(area => area.AreaId).ToList();
                var result = await membershipRequestRepository.GetFedMembershipAnalyticsAsync(tenantID, areaIds, typeId);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetFedEntityAnalytics>("Something went wrong");
            }
        }
        public async Task<Response<GetBatchAnalytics>> GetBatchAnalytics(int batchId, int typeId)
        {
            try
            {

                var result = await membershipRequestRepository.GetBatchAnalyticsAsync(batchId, typeId);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetBatchAnalytics>("Something went wrong");
            }
        }

        public async Task<Response<List<GetAllUserMembershipRequests>>> GetCurrentUserMembershipRequestsAsync()
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();

                Expression<Func<MembershipRequest, bool>> criteria =
                    r => r.UserId == userId;

                var spec = new GeneralSpecifications<MembershipRequest>(criteria);
                spec.AddInclude(entity => entity.Include(entity => entity.Tenant));
                spec.AddInclude(entity => entity.Include(entity => entity.RequestType));
                spec.AddInclude(entity => entity.Include(entity => entity.SubMembers).ThenInclude(s => s.RequestType));
                spec.SetOrderByDescending(r => r.CreationDate);

                var entities = await unitOfWork.Repository<MembershipRequest>()
                                               .GetAllWithSpecifications(spec);

                if (entities == null || !entities.Any())
                    return new ResponseHandler().Success(new List<GetAllUserMembershipRequests>());

                // Flatten results:
                // - Keep all requests except type 21
                // - Replace type 21 requests with their submembers
                var flattened = new List<MembershipRequest>();

                foreach (var req in entities)
                {
                    // Type != 21,non-parent → regular self request
                    if (req.RequestTypeId != 21  && req.IsParent == false)
                    {
                        flattened.Add(req);
                        continue;
                    }

                    // Type == 21, IsParent == true → don't add the parent itself,
                    // instead add its sub-members (which carry the child's data)
                    if (req.RequestTypeId == 21 && req.IsParent == true && req.Status != RequestStatus.Draft)
                    {
                        if (req.SubMembers?.Any() == true)
                            flattened.AddRange(req.SubMembers);
                        // else: parent with no submembers yet — optionally add the parent itself as fallback
                        continue;
                    }
                }

                // Map to DTO
                var dto = mapper.Map<List<GetAllUserMembershipRequests>>(flattened);

                var tasks = dto.Select(async dtoItem =>
                {
                    dtoItem.TenantImage = await DocumentSetting.GetPreSignedUrlAsync(dtoItem.TenantImage);
                    dtoItem.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(dtoItem.UserPhoto);
                }).ToList();
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(dto);
            }
            catch (Exception)
            {
                return new ResponseHandler().BadRequest<List<GetAllUserMembershipRequests>>("Something went wrong");
            }
        }
public async Task<Response<List<MembershipRequestWithFormDataDto>>> GetCurrentUserMembershipRequestsWithChildrenAsync(
    int requestId,
    int? childRequestId = null)
{
    try
    {
        var userId = httpContextAccessor.HttpContext.User.GetUserId();
        var cacheKey = $"MembershipRequests_Batch_{requestId}_{childRequestId}_{userId}";

        if (memoryCache.TryGetValue(
            cacheKey,
            out List<MembershipRequestWithFormDataDto> cachedRequests))
        {
            return new ResponseHandler().Success(cachedRequests);
        }

        // Get the requested request
        var requestedRequest = await unitOfWork.Repository<MembershipRequest>()
            .GetByIdAsync(requestId);

        if (requestedRequest == null)
        {
            return new ResponseHandler()
                .NotFound<List<MembershipRequestWithFormDataDto>>(
                    "Membership request not found.");
        }

        // A child request is not loaded by GetByIdAsync (FindAsync), so do not
        // rely on requestedRequest.SubMembers to locate a requested child.
        // Query the child directly and ensure it belongs to the supplied parent.
        Expression<Func<MembershipRequest, bool>> criteria;

        if (childRequestId.HasValue)
        {
            criteria = r =>
                r.Id == childRequestId.Value &&
                r.SuperMembershipRequestId == requestId;
        }
        else
        {
            // IsParent is persisted on the request and does not require loading
            // the SubMembers navigation property.
            criteria = requestedRequest.IsParent
                ? r => r.SuperMembershipRequestId == requestId
                : r => r.Id == requestId;
        }

        var spec = new GeneralSpecifications<MembershipRequest>(criteria);

        spec.SetOrderByDescending(r => r.CreationDate);
        spec.AsSplitQuery = true;
        spec.AsNoTracking = true;

        var requests = await unitOfWork.Repository<MembershipRequest>()
            .GetAllWithSpecificationsAndProjection(
                spec,
                m => new MembershipRequestWithFormDataDto
                {
                    Id = m.Id,
                    Status = m.Status,
                    BatchNumber = m.Batch != null
                        ? m.Batch.BatchNumber
                        : null,
                    CreationDate = m.CreationDate,
                    FileNumber = m.FileNumber != null
                        ? m.FileNumber
                        : "N/A",
                    FirstName = m.FirstName != null
                        ? m.FirstName
                        : "N/A",
                    LastName = m.LastName != null
                        ? m.LastName
                        : "N/A",
                    UserPhoto = m.UserPhoto != null
                        ? m.UserPhoto
                        : "N/A",
                    PackageId = m.PackageId,
                    PackageName = m.Package != null
                        ? m.Package.Name
                        : "N/A",
                    TenantId = m.TenantId,
                    TenantName = m.Tenant != null
                        ? m.Tenant.Name
                        : null,

                    LastStatusUpdate = m.RequestStatusHistories!
                        .OrderByDescending(rh => rh.CreationDate)
                        .Select(rh => (DateTime?)rh.CreationDate)
                        .FirstOrDefault(),

                    RequestType = m.RequestType != null
                        ? new RequestTypeDto
                        {
                            Id = m.RequestType.Id,
                            Name = m.RequestType.Name,
                            Description = m.RequestType.Description,
                            RequestTypeCategoryId =
                                m.RequestType.RequestTypeCategoryId
                        }
                        : new RequestTypeDto(),

                    Package = m.Package != null
                        ? new DTOs.Entity.PackageDtos.PackageSummaryDto
                        {
                            Id = m.Package.Id,
                            Name = m.Package.Name,
                            Duration = m.Package.Duration,
                            Image = m.Package.Image,
                            Price = m.Package.Price,
                            BranchCount = m.Package.Branches != null
                                ? m.Package.Branches.Count()
                                : 0,
                            FacilityCount = m.Package.Facilities != null
                                ? m.Package.Facilities.Count()
                                : 0,
                            VenueCount = m.Package.Venues != null
                                ? m.Package.Venues.Count()
                                : 0
                        }
                        : new DTOs.Entity.PackageDtos.PackageSummaryDto(),

                    AccessTree = new MembershipRequestAccessTreeDto
                    {
                        SelectedBranches =
                            m.MembershipRequestAccesses != null
                                ? m.MembershipRequestAccesses.Select(a => a.BranchId)
                                : new List<int>(),

                        SelectedFacilities =
                            m.MembershipRequestAccesses != null
                                ? m.MembershipRequestAccesses
                                    .Where(a => a.FacilityId.HasValue)
                                    .Select(a => a.FacilityId.Value)
                                : new List<int>(),

                        SelectedVenues =
                            m.MembershipRequestAccesses != null
                                ? m.MembershipRequestAccesses
                                    .Where(a => a.VenueId.HasValue)
                                    .Select(a => a.VenueId.Value)
                                : new List<int>()
                    },

                    FormFees =
                        m.FormUserResponse != null &&
                        m.FormUserResponse.Form != null
                            ? m.FormUserResponse.Form.Fees
                            : 0,

                    FormUserResponse = m.FormUserResponse != null
                        ? new GetParentFormUserResponseDto
                        {
                            UserId = m.FormUserResponse.UserId,

                            FormId = m.FormUserResponse.Form != null
                                ? m.FormUserResponse.Form.Id
                                : 0,

                            FieldsResponse =
                                m.FormUserResponse.FieldsResponse != null
                                    ? m.FormUserResponse.FieldsResponse
                                        .Select(fr => new ParentFormFieldResponseDto
                                        {
                                            FieldName = fr.Field != null
                                                ? fr.Field.Title
                                                : "N/A",
                                            FieldId = fr.FieldId,
                                            Response = fr.Response
                                        })
                                        .ToList()
                                    : new List<ParentFormFieldResponseDto>()
                        }
                        : new GetParentFormUserResponseDto(),

                    // We return the actual request itself,
                    // so there are no nested children.
                    IsParent = false,

                    SubMembers = new List<MembershipRequestWithFormDataDto>()
                });

        if (!requests.Any())
        {
            return new ResponseHandler()
                .Success(new List<MembershipRequestWithFormDataDto>());
        }

        // Assign field indices and hidden status
        foreach (var request in requests)
        {
            if (request.Package != null && request.AccessTree != null)
            {
                request.AccessTree.SelectedVenues =
                    request.AccessTree.SelectedVenues
                        .Distinct()
                        .ToList();

                request.AccessTree.SelectedBranches =
                    request.AccessTree.SelectedBranches
                        .Distinct()
                        .ToList();

                request.AccessTree.SelectedFacilities =
                    request.AccessTree.SelectedFacilities
                        .Distinct()
                        .ToList();

                request.Package.VenueCount =
                    request.AccessTree.SelectedVenues.Distinct().Count();

                request.Package.BranchCount =
                    request.AccessTree.SelectedBranches.Distinct().Count();

                request.Package.FacilityCount =
                    request.AccessTree.SelectedFacilities.Distinct().Count();
            }

            if (request.FormUserResponse != null &&
                request.FormUserResponse.FormId > 0)
            {
                var formFields =
                    await unitOfWork.Repository<FormFields>()
                        .GetAllAsync(ff =>
                            ff.FormId == request.FormUserResponse.FormId);

                foreach (var field in request.FormUserResponse.FieldsResponse)
                {
                    var match = formFields.FirstOrDefault(ff =>
                        ff.FieldId == field.FieldId);

                    field.Index = match?.Index ?? 0;
                    field.IsHidden = match?.IsHidden ?? false;
                }
            }
        }

        if (childRequestId.HasValue &&
            requests.Any(r => r.Status == RequestStatus.RequestResolved))
        {
            memoryCache.Set(
                cacheKey,
                requests,
                TimeSpan.FromMinutes(10));
        }

        // Generate pre-signed URLs
        var tasks = requests.Select(async dtoItem =>
        {
            if (!string.IsNullOrEmpty(dtoItem.UserPhoto) &&
                dtoItem.UserPhoto != "N/A")
            {
                dtoItem.UserPhoto =
                    await DocumentSetting.GetPreSignedUrlAsync(
                        dtoItem.UserPhoto);
            }

            if (dtoItem.Package != null &&
                !string.IsNullOrEmpty(dtoItem.Package.Image))
            {
                dtoItem.Package.Image =
                    await DocumentSetting.GetPreSignedUrlAsync(
                        dtoItem.Package.Image);
            }
        }).ToList();

        await Task.WhenAll(tasks);

        return new ResponseHandler()
            .Success(requests);
    }
    catch (Exception ex)
    {
        Log.Error(
            ex,
            "Error getting membership requests. RequestId: {RequestId}, ChildRequestId: {ChildRequestId}",
            requestId,
            childRequestId);

        return new ResponseHandler()
            .BadRequest<List<MembershipRequestWithFormDataDto>>(
                "Something went wrong");
    }
}
        public async Task<Response<List<MembershipRequestWithFormDataDto>>> GetCurrentUserMembershipRequestsWithChildrenByBatchAsync(int batchId)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();

                Expression<Func<MembershipRequest, bool>> criteria =
                    r => r.BatchId == batchId &&( r.IsParent || (r.RequestTypeId == 6 && !r.SubMembers.Any()));

                var spec = new GeneralSpecifications<MembershipRequest>(criteria);
                spec.SetOrderByDescending(r => r.CreationDate);
                spec.AsSplitQuery = true;
                spec.AsNoTracking = true;
                var parents = await unitOfWork.Repository<MembershipRequest>()
                          .GetAllWithSpecificationsAndProjection(spec, m => new MembershipRequestWithFormDataDto
                          {
                              Id = m.Id,
                              Status = m.Status,
                              BatchNumber = m.Batch != null ? m.Batch.BatchNumber : null,
                              CreationDate = m.CreationDate,
                              FileNumber = m.FileNumber != null ? m.FileNumber : "N/A",
                              FirstName = m.FirstName != null ? m.FirstName : "N/A",
                              LastName = m.LastName != null ? m.LastName : "N/A",
                              UserPhoto = m.UserPhoto != null ? m.UserPhoto : "N/A",
                              PackageId = m.PackageId,
                              PackageName = m.Package != null ? m.Package.Name : "N/A",
                              TenantId = m.TenantId,
                              TenantName = m.Tenant != null ? m.Tenant.Name : null,
                              LastStatusUpdate = m.RequestStatusHistories!
                                  .OrderByDescending(rh => rh.CreationDate)
                                  .Select(rh => (DateTime?)rh.CreationDate)
                                  .FirstOrDefault(),

                              RequestType = m.RequestType != null ? new RequestTypeDto
                              {
                                  Id = m.RequestType.Id,
                                  Name = m.RequestType.Name,
                                  Description = m.RequestType.Description,
                                  RequestTypeCategoryId = m.RequestType.RequestTypeCategoryId
                              } : new RequestTypeDto(),

                              Package = m.Package != null ? new DTOs.Entity.PackageDtos.PackageSummaryDto
                              {
                                  Id = m.Package.Id,
                                  Name = m.Package.Name,
                                  Duration = m.Package.Duration,
                                  Image = m.Package.Image,
                                  Price = m.Package.Price
                              } : new DTOs.Entity.PackageDtos.PackageSummaryDto(),

                              AccessTree = new MembershipRequestAccessTreeDto
                              {
                                  SelectedBranches = m.MembershipRequestAccesses != null ? m.MembershipRequestAccesses.Select(a => a.BranchId) : new List<int>(),
                                  SelectedFacilities = m.MembershipRequestAccesses != null ? m.MembershipRequestAccesses.Where(a => a.FacilityId.HasValue).Select(a => a.FacilityId.Value) : new List<int>(),
                                  SelectedVenues = m.MembershipRequestAccesses != null ? m.MembershipRequestAccesses.Where(a => a.VenueId.HasValue).Select(a => a.VenueId.Value) : new List<int>()
                              },

                              FormFees = m.FormUserResponse != null && m.FormUserResponse.Form != null
                                  ? m.FormUserResponse.Form.Fees
                                  : 0,

                              FormUserResponse = m.FormUserResponse != null
                                  ? new GetParentFormUserResponseDto
                                  {
                                      UserId = m.FormUserResponse.UserId,
                                      FormId = m.FormUserResponse.Form != null ? m.FormUserResponse.Form.Id : 0,
                                      FieldsResponse = m.FormUserResponse.FieldsResponse != null
                                          ? m.FormUserResponse.FieldsResponse.Select(fr => new ParentFormFieldResponseDto
                                          {
                                              FieldName = fr.Field != null ? fr.Field.Title : "N/A",
                                              FieldId = fr.FieldId,
                                              Response = fr.Response
                                          }).ToList()
                                          : new List<ParentFormFieldResponseDto>()
                                  }
                                  : new GetParentFormUserResponseDto(),

                              SubMembers = m.SubMembers != null
                                  ? m.SubMembers.Select(sm => new MembershipRequestWithFormDataDto
                                  {
                                      Id = sm.Id,
                                      Status = sm.Status,
                                      CreationDate = sm.CreationDate,
                                      FileNumber = sm.FileNumber != null ? sm.FileNumber : "N/A",
                                      FirstName = sm.FirstName != null ? sm.FirstName : "N/A",
                                      LastName = sm.LastName != null ? sm.LastName : "N/A",
                                      UserPhoto = sm.UserPhoto != null ? sm.UserPhoto : "N/A",
                                      PackageId = sm.PackageId,
                                      PackageName = sm.Package != null ? sm.Package.Name : "N/A",
                                      Package = sm.Package != null ? new DTOs.Entity.PackageDtos.PackageSummaryDto
                                      {
                                          Id = sm.Package.Id,
                                          Name = sm.Package.Name,
                                          Duration = sm.Package.Duration,
                                          Image = sm.Package.Image,
                                          Price = sm.Package.Price,
                                      } : new DTOs.Entity.PackageDtos.PackageSummaryDto(),
                                      TenantId = sm.TenantId,
                                      TenantName = sm.Tenant != null ? sm.Tenant.Name : null,
                                      AccessTree = new MembershipRequestAccessTreeDto
                                      {
                                          SelectedBranches = sm.MembershipRequestAccesses != null ? sm.MembershipRequestAccesses.Select(a => a.BranchId) : new List<int>(),
                                          SelectedFacilities = sm.MembershipRequestAccesses != null ? sm.MembershipRequestAccesses.Where(a => a.FacilityId.HasValue).Select(a => a.FacilityId.Value) : new List<int>(),
                                          SelectedVenues = sm.MembershipRequestAccesses != null ? sm.MembershipRequestAccesses.Where(a => a.VenueId.HasValue).Select(a => a.VenueId.Value) : new List<int>()
                                      },
                                      FormUserResponse = sm.FormUserResponse != null
                                          ? new GetParentFormUserResponseDto
                                          {
                                              UserId = sm.FormUserResponse.UserId,
                                              FormId = sm.FormUserResponse.Form != null ? sm.FormUserResponse.Form.Id : 0,
                                              FieldsResponse = sm.FormUserResponse.FieldsResponse != null
                                                  ? sm.FormUserResponse.FieldsResponse.Select(fr => new ParentFormFieldResponseDto
                                                  {
                                                      FieldName = fr.Field != null ? fr.Field.Title : "N/A",
                                                      FieldId = fr.FieldId,
                                                      Response = fr.Response
                                                  }).ToList()
                                                  : new List<ParentFormFieldResponseDto>()
                                          }
                                          : new GetParentFormUserResponseDto(),
                                      SubMembers = new List<MembershipRequestWithFormDataDto>()
                                  }).ToList()
                                  : new List<MembershipRequestWithFormDataDto>()
                          });
                foreach (var parent in parents)
                {
                    parent.AccessTree.SelectedVenues = parent.AccessTree.SelectedVenues.Distinct().ToList();
                    parent.AccessTree.SelectedBranches = parent.AccessTree.SelectedBranches.Distinct().ToList();
                    parent.AccessTree.SelectedFacilities = parent.AccessTree.SelectedFacilities.Distinct().ToList();
                    if (parent.Package != null)
                    {

                        parent.Package.VenueCount = parent.AccessTree.SelectedVenues.Distinct().Count();
                        parent.Package.BranchCount = parent.AccessTree.SelectedBranches.Distinct().Count();
                        parent.Package.FacilityCount = parent.AccessTree.SelectedFacilities.Distinct().Count();

                    }
                    var formFields = await unitOfWork.Repository<FormFields>()
                          .GetAllAsync(ff => ff.FormId == parent.FormUserResponse.FormId);

                    if (parent.FormUserResponse != null)
                    {


                        foreach (var field in parent.FormUserResponse.FieldsResponse)
                        {
                            var match = formFields.FirstOrDefault(ff => ff.FieldId == field.FieldId);
                            field.Index = match?.Index ?? 0;
                            field.IsHidden = match?.IsHidden ?? false;
                        }
                    }

                    foreach (var sm in parent.SubMembers)
                    {
                        sm.AccessTree.SelectedVenues = sm.AccessTree.SelectedVenues.Distinct().ToList();
                        sm.AccessTree.SelectedBranches = sm.AccessTree.SelectedBranches.Distinct().ToList();
                        sm.AccessTree.SelectedFacilities = sm.AccessTree.SelectedFacilities.Distinct().ToList();
                        if (sm.Package != null)
                        {

                            sm.Package.VenueCount = sm.AccessTree.SelectedVenues.Distinct().Count();
                            sm.Package.BranchCount = sm.AccessTree.SelectedBranches.Distinct().Count();
                            sm.Package.FacilityCount = sm.AccessTree.SelectedFacilities.Distinct().Count();

                        }
                        if (sm.FormUserResponse != null)
                        {


                            foreach (var field in sm.FormUserResponse.FieldsResponse)
                            {
                                var match = formFields.FirstOrDefault(ff => ff.FieldId == field.FieldId);
                                field.Index = match?.Index ?? 0;
                                field.IsHidden = match?.IsHidden ?? false;
                            }
                        }
                    }
                }

                var tasks = parents.Select(async dtoItem =>
                {
                    dtoItem.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(dtoItem.UserPhoto);

                    if (dtoItem.Package != null)
                        dtoItem.Package.Image = await DocumentSetting.GetPreSignedUrlAsync(dtoItem.Package.Image);
                }).ToList();

                var subTasks = parents.SelectMany(p => p.SubMembers).Select(async dtoItem =>
                {
                    dtoItem.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(dtoItem.UserPhoto);
                    if (dtoItem.Package != null)
                        dtoItem.Package.Image = await DocumentSetting.GetPreSignedUrlAsync(dtoItem.Package.Image);
                }).ToList();

                await Task.WhenAll(tasks.Concat(subTasks).ToList());

                return new ResponseHandler().Success(parents);
            }
            catch (Exception ex)
            {
                return new ResponseHandler()
                    .BadRequest<List<MembershipRequestWithFormDataDto>>("Something went wrong: " + ex.Message);
            }
        }
    }
}
