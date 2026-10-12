using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.ConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.ConfigurationRepo;
using MyEntityMembershipService.Repositories.Membership.FieldRepo;
using MyEntityMembershipService.Repositories.Membership.FormRepo;
using MyEntityMembershipService.Repositories.Membership.FormUserResponseRepo;
using MyEntityMembershipService.Repositories.Membership.MembershipRepo;
using MyEntityMembershipService.Repositories.Membership.MembershipRequestRepository;
using MyEntityMembershipService.Repositories.Membership.MemberTypeRepo;
using MyEntityMembershipService.Repositories.Membership.PaakageMemberFormRepo;
using MyEntityMembershipService.Repositories.Membership.PackageRequestAuthourizationsRepo;
using MyEntityMembershipService.Repositories.Membership.RequestTypeRepo;
using MyEntityMembershipService.Repositories.Membership.UnitRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FreezeConfigurationService;
using MyEntityMembershipService.Services.Membership.GetConfigurationService;
using MyEntityMembershipService.SpecificationPattern;
using Newtonsoft.Json;
using Serilog;
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace MyEntityMembershipService.Services.Membership.FormServ
{
    public class FormService : IFormService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IFormRepository formRepository;
        private readonly IRequestTypeRepository requestTypeRepository;
        private readonly IFieldRepository fieldRepository;
        private readonly IPackageRequestAuthorizationRepository packageRequestAuthorizationRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IMembershipRepository membershipRepository;
        private readonly IConfigurationRepository configurationRepository;
        private readonly IMembershipRequestRepository membershipRequestRepository;
        private readonly IUnitRepository unitRepository;
        private readonly IFreezeConfigurationService freezeConfigurationService;
        private readonly IMemberTypeRepo memberTypeRepo;
        private readonly IGetConfigurationService getConfigurationService;
        private readonly IPaakageMemberFormRepo paakageMemberFormRepo;
        private readonly IConfiguration configuration;
        private readonly IFormUserResponseRepository formUserResponseRepository;

        public FormService(IUnitOfWork unitOfWork, IMapper mapper, IFormRepository formRepository,
                            IRequestTypeRepository requestTypeRepository, IFieldRepository fieldRepository,
                            IConfiguration configuration, IFormUserResponseRepository formUserResponseRepository,
                            IPackageRequestAuthorizationRepository packageRequestAuthorizationRepository, IHttpContextAccessor httpContextAccessor,
                            IBackgroundJobClient backgroundJobClient, IMembershipRepository membershipRepository,
                            IConfigurationRepository configurationRepository, IMembershipRequestRepository membershipRequestRepository,
                            IUnitRepository unitRepository, IFreezeConfigurationService freezeConfigurationService,
                            IMemberTypeRepo memberTypeRepo , IGetConfigurationService getConfigurationService,
                            IPaakageMemberFormRepo paakageMemberFormRepo  
                            )
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.formRepository = formRepository;
            this.requestTypeRepository = requestTypeRepository;
            this.fieldRepository = fieldRepository;
            this.configuration = configuration;
            this.formUserResponseRepository = formUserResponseRepository;
            this.packageRequestAuthorizationRepository = packageRequestAuthorizationRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.backgroundJobClient = backgroundJobClient;
            this.membershipRepository = membershipRepository;
            this.configurationRepository = configurationRepository;
            this.membershipRequestRepository = membershipRequestRepository;
            this.unitRepository = unitRepository;
            this.freezeConfigurationService = freezeConfigurationService;
            this.memberTypeRepo = memberTypeRepo;
            this.getConfigurationService = getConfigurationService;
            this.paakageMemberFormRepo = paakageMemberFormRepo;
        }
        public async Task<Response<List<RequestTypeFieldsGroupedByCategoryDto>>> GetRequestTypeFields(int requestTypeId)
        {
            try
            {

                var flatFields = await unitOfWork.Repository<RequestTypeFields>()
                    .GetAllWithSpecificationsAndProjection(
                    new GeneralSpecifications<RequestTypeFields>(rtf => rtf.RequestTypeId == requestTypeId),
                    rtf => new
                    {
                        rtf.FieldCategoryId,
                        FieldCategoryName = rtf.FieldCategory.Name,
                        Field = new FieldDto
                        {
                            Id = rtf.Field.Id,
                            Title = rtf.Field.Title,
                            TitleAr = rtf.Field.TitleAr,
                            FieldTypeId = rtf.Field.FieldTypeId,
                            FieldTypeName = rtf.Field.FieldType.Name,
                            IsRequired = rtf.IsRequired,
                            IsEditable = rtf.IsEditable,
                            FieldCategoryId = rtf.FieldCategoryId,
                            IsChecked = rtf.IsChecked,
                            DropdownValuesDto = rtf.Field.DropdownValues != null
                                ? rtf.Field.DropdownValues.Select(dv => dv.Value).ToList()
                                : new List<string>(),
                            FieldUnits = rtf.Field.UnitType != null
                            ? new List<FieldUnitsDto> { new FieldUnitsDto { Id = rtf.Field.UnitType.Id, Name = rtf.Field.UnitType.Name } }
                            : new List<FieldUnitsDto>()
                    }
                        });


                Log.Information("Retrieved {FieldCount} fields for RequestTypeId {RequestTypeId}", flatFields.Count, requestTypeId);



                var response = flatFields
                    .GroupBy(x => new { x.FieldCategoryId, x.FieldCategoryName })
                    .Select(g => new RequestTypeFieldsGroupedByCategoryDto
                    {
                        FieldCategoryId = g.Key.FieldCategoryId,
                        FieldCategoryName = g.Key.FieldCategoryName,
                        Fields = g.Select(x => x.Field).ToList()
                    })
                    .ToList();

                Log.Information("Grouped fields into {CategoryCount} categories for RequestTypeId {RequestTypeId}", response.Count, requestTypeId);

                return new ResponseHandler().Success(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetRequestTypeFields failed for RequestTypeId {RequestTypeId}", requestTypeId);
                return new ResponseHandler().BadRequest<List<RequestTypeFieldsGroupedByCategoryDto>>(ex.Message);
            }
        }
        

        public async Task<Response<List<ReturnFormDto>>> GetFormsCreatedBy(string userId, int requestTypeId, string? title, int? tenantId)
        {
            try
            {
                if (!tenantId.HasValue)
                {
                    Log.Error("Tenant Id is required");
                    return new ResponseHandler().BadRequest<List<ReturnFormDto>>("Tenant Id is required");

                }

                var trimmedTitle = title?.Trim().ToLower();

                Expression<Func<Form, bool>> criteria = f =>
                    f.RequestTypeId == requestTypeId &&
                    f.TenantId == tenantId.Value &&
                    (string.IsNullOrEmpty(trimmedTitle) || (f.Title != null && f.Title.Trim().ToLower().Contains(trimmedTitle)));


                var formSpec = new GeneralSpecifications<Form>(criteria);
                formSpec.AddInclude(q => q.Include(f => f.RequestType));
                formSpec.SetOrderByDescending(f => f.Id);
                var forms = await unitOfWork.Repository<Form>().GetAllWithSpecifications(formSpec);

                var result = mapper.Map<List<ReturnFormDto>>(forms);
                foreach (var form in forms)
                {
                    var item = result.Find(f => f.Id == form.Id);
                    item.RequestTypeName = form.RequestType.Name;
                    var client = new HttpClient();
                    if (!string.IsNullOrEmpty(form.CreatedBy))
                    {
                        var response = await client.GetStringAsync($"{configuration["ApiSettings:Gateway"]}{configuration["MySelfOldUrl:GetUsers"]}?userId={form.CreatedBy}");
                        var userData = JsonConvert.DeserializeObject<UserDataDto>(response);
                        item.UserName = userData.UserName;
                    }
                }
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<ReturnFormDto>>(ex.Message);
            }
        }

        public async Task<Response<ReturnFormDto>> GetForm(int id, int? packageId , int? memberTypeId)
        {
            try
            {
                var form = await formRepository.GetForm(id);
                if (form is null)
                    return new ResponseHandler().BadRequest<ReturnFormDto>("There is no form with this Id");
                var formDto = mapper.Map<ReturnFormDto>(form);
                formDto.RequestTypeName = form.RequestType?.Name;
                formDto.Fields = new List<FieldDto>();
                foreach (var item in form.FormFields)
                {
                    var field = await fieldRepository.GetFieldWithDropdownValues(item.FieldId);
                    var fieldDto = mapper.Map<FieldDto>(field);
                    fieldDto.FieldTypeName = field.FieldType.Name;
                    fieldDto.IsRequired = item.IsRequired;
                    fieldDto.IsEditable = item.IsEditable;

                    // if The field is dropdown => get dropdown Values
                    if (field.DropdownValues.Count > 0 && field.FieldTypeId == 2)
                        fieldDto.DropdownValuesDto = field.DropdownValues.Select(dropdownValue => dropdownValue.Value).ToList();
                    if (packageId.HasValue)
                    {
                        // Get dropdown values of reasons of cancellation && removal && separation 
                        //var configuration = await configurationRepository.GetPackageDetailsByRequestTypeAndPackageID(form.RequestTypeId, packageId.Value);
                        //if (field.DropdownValues.Count == 0 && field.FieldTypeId == 2)
                        //{
                        //    // For Cancellation
                        //    if (form.RequestTypeId == 17)
                        //        fieldDto.DropdownValuesDto = configuration.Where(config => config.FieldHeader == "List of potential reasons for cancellation").Select(config => config.FieldName).ToList();
                        //    // For separation
                        //    if (form.RequestTypeId == 9)
                        //        fieldDto.DropdownValuesDto = configuration.Where(config => config.FieldHeader == "List of potential reasons for Separation").Select(config => config.FieldName).ToList();
                        //    // For Removal
                        //    if (form.RequestTypeId == 18)
                        //        fieldDto.DropdownValuesDto = configuration.Where(config => config.FieldHeader == "List of potential reasons for Removal").Select(config => config.FieldName).ToList();
                        //}
                        //switch (form.RequestTypeId)
                        //{
                        //    case 17:
                        //        if (field.Title == "Cancellation Fee")
                        //            fieldDto.DefaultValue = configuration.Where(config => config.FieldHeader == "Cancellation Fees").FirstOrDefault()?.FieldResponseString;
                        //        break;
                        //    case 9:
                        //        if (field.Title == "Separation Fee")
                        //            fieldDto.DefaultValue = configuration.Where(config => config.FieldHeader == "Separation Fees").FirstOrDefault()?.FieldResponseString;
                        //        break;
                        //    case 18:
                        //        if (field.Title == "Removal Fee")
                        //            fieldDto.DefaultValue = configuration.Where(config => config.FieldHeader == "Removal Fees").FirstOrDefault()?.FieldResponseString;
                        //        break;
                        //}
                    }
                    if (field.FieldTypeId == 10 && field.UnitTypeId.HasValue)
                    {
                        var units = await unitRepository.GetByUnitTypeIdWithTypeNameAsync(field.UnitTypeId.Value);
                        fieldDto.FieldUnits = mapper.Map<List<FieldUnitsDto>>(units);
                    }
                    formDto.Fields.Add(fieldDto);
                }
                if (packageId is not null && memberTypeId is not null)
                {
                    // Get Operational Fees in case of Addition Request or Removal
                    formDto.OperationalFees = await GetOperationalFees(form.RequestTypeId, packageId.Value, memberTypeId.Value);


                    // To Get Form Fees updated from Configuration According to SubMember Member Type
                    var packageMemberForm = await paakageMemberFormRepo.GetMemberFormByFormIdPackageIdmemberTypeId(packageId.Value, id, memberTypeId.Value);
                    formDto.UpdatedFees = packageMemberForm.Fees.Value;

                }


                return new ResponseHandler().Success(formDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<ReturnFormDto>(ex.Message);
            }
        }
        // Get Operational Fees in case of Addition Request or Removal
        public async Task<double> GetOperationalFees(int RequestTypeId , int PackageId , int MemberTypeId)
        {
            if (RequestTypeId == 16 || RequestTypeId == 18)
            {
                var Config = await getConfigurationService.GetConfiguartionAsync(PackageId, RequestTypeId, MemberTypeId);
                if (RequestTypeId == 16)
                {
                    var additionalConfig = (GetAdditionalConfigurationDto)Config?.Data;
                    return additionalConfig.MembershipAdditionFormFees.Value;
                }
                else
                {
                    var removalConfig = (GetRemovalConfigurationDto)Config?.Data;
                    return removalConfig.RemovalFees.Value;
                }

            }
            return 0;
        }

        public async Task<Response<List<ReturnRequestTypesGrouped>>> GetRequestTypesGroupedByCategories(int? tenantId)
        {
            try
            {
                if (!tenantId.HasValue)
                {
                    Log.Warning("GetRequestTypesGroupedByCategories failed — TenantId is missing.");
                    return new ResponseHandler().BadRequest<List<ReturnRequestTypesGrouped>>("TenantId is required.");
                }

                Expression<Func<RequestTypeCategory, ReturnRequestTypesGrouped>> categorySelector = c => new ReturnRequestTypesGrouped
                {
                    Id = c.Id,
                    Name = c.Name,
                    RequestTypes = c.RequestTypes.Select(t => new ReturnRequestTypeDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        NumberOfForms = t.Forms.Count(f => f.TenantId == tenantId.Value && f.ParentFormId == null)
                    }).ToList()
                };

                var categorySpec = new GeneralSpecifications<RequestTypeCategory>() { AsNoTracking = true };
                var categoriesDto = await unitOfWork.Repository<RequestTypeCategory>()
                    .GetAllWithSpecificationsAndProjection(categorySpec, categorySelector);

                Log.Information("GetRequestTypesGroupedByCategories successfully retrieved {CategoryCount} categories for TenantId {TenantId}", categoriesDto.Count, tenantId.Value);

                return new ResponseHandler().Success(categoriesDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetRequestTypesGroupedByCategories threw an exception. TenantId: {TenantId}", tenantId);
                return new ResponseHandler().BadRequest<List<ReturnRequestTypesGrouped>>("An unexpected error occurred.");
            }
        }

        public async Task<Response<ReturnFormDto>> DeleteForm(int id)
        {
            try
            {
                var form = await unitOfWork.Repository<Form>().GetByIdAsync(id);
                unitOfWork.Repository<Form>().Delete(form);
                await unitOfWork.Save();
                var formDto = mapper.Map<ReturnFormDto>(form);
                return new ResponseHandler().Success(formDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<ReturnFormDto>(ex.Message);
            }
        }
        public async Task<Response<ReturnFormUserResponseDto>> AddFormUserResponse(FormUserResponseDto formUserResponseDto)
        {
            try
            {
                var form = await formRepository.GetForm(formUserResponseDto.FormId);
                if (form is null)
                    return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>("There is no form with that Id");
                if (form.WorkflowSelection == "Direct Action" && form.RequestTypeId == 6 && formUserResponseDto.SubMembers?.Count > 0)
                    return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>("You Can't add submembers in case of Direct Action");
                Data.Membership.Entities.Membership membership = null;
                Data.Membership.Entities.Membership submembership = null;
                if(!formUserResponseDto.PackageId.HasValue && form.RequestTypeId == 6)
                {
                    return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>("Package Id is Required");
                }
                if (formUserResponseDto.MembershipId.HasValue)
                {
                    membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(formUserResponseDto.MembershipId.Value);
                    if (membership is null)
                        return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>("There is no membership with that Id");
                }
                if (formUserResponseDto.SubMemberId.HasValue)
                {
                    submembership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(formUserResponseDto.SubMemberId.Value);
                    if (submembership is null)
                        return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>("There is no membership for that Submember");
                }
                if (form.RequestTypeId == 18 && !formUserResponseDto.SubMemberId.HasValue)
                    return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>("You must select submember to remove");
                if (membership is not null && form.RequestTypeId == 17)
                {
                    if (membership.ParentMembershipId.HasValue)
                        return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>("You have to do cancellation on the main membership not submember");
                }
                var formFields = form.FormFields.Select(item => item.FieldId).ToList();
                foreach (var fieldResponse in formUserResponseDto.FieldsResponse)
                {
                    // Check that form contain this field
                    if (!formFields.Contains(fieldResponse.FieldId))
                        return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>($"There is no field with that Id = {fieldResponse.FieldId}");
                    // Required Validation Checking
                    var field = form.FormFields.FirstOrDefault(field => field.FieldId == fieldResponse.FieldId);
                    if (field.IsRequired.Value && string.IsNullOrEmpty(fieldResponse.Response))
                    {
                        var fieldData = await unitOfWork.Repository<Field>().GetByIdAsync(field.FieldId);
                        return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>($"{fieldData.Title} is Required");
                    }
                }
                var formUserResponse = mapper.Map<FormUserResponse>(formUserResponseDto);
                if (form.RequestTypeId == 12)
                {
                    if (membership.ParentMembershipId.HasValue && membership.CreatedBy != formUserResponseDto.UserId)
                        return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>("you can't freez yourself , only your Main Member can freez you");
                    var freezData = await GetFreezDuration(formUserResponse);
                    var freezConfig = await freezeConfigurationService.GetByPackageId(membership.PackageId.Value);
                    if (freezConfig.Data.CanExceed != true && freezData.Data.FreezDuration > freezConfig.Data.FreezeLimit)
                        return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>($"You can't exceed your package freeze Limit = {freezConfig.Data.FreezeLimit}");
                }
                //TODO: Fill MemberId
                await unitOfWork.Repository<FormUserResponse>().Add(formUserResponse);
                await unitOfWork.Save();
                var model = new CreateMembershipRequestForResponseDto()
                {
                    Form = form,
                    FormUserResponseId = formUserResponse.Id,
                    FormUserResponseDto = formUserResponseDto,
                    Membership = membership,
                    SubMembership = submembership
                };
                // For Case of creating New Membership Request at the same request Adding Members
                if (formUserResponseDto.SubMembers is not null && (form.RequestTypeId == 6 || form.RequestTypeId == 16 || form.RequestTypeId == 18))
                {
                    var SubMembersFormUserResponse = mapper.Map<List<FormUserResponse>>(formUserResponseDto.SubMembers);
                    unitOfWork.Repository<FormUserResponse>().AddRange(SubMembersFormUserResponse);
                    await unitOfWork.Save();
                    model.SubMembersFormUserResponseId = SubMembersFormUserResponse.Select(res => res.Id).ToList();
                    model.SubMembersFormUserResponseDto = mapper.Map<List<FormUserResponseDto>>(SubMembersFormUserResponse);
                }
                await CreateMembershipRequestForResponse(model);
                formUserResponseDto = mapper.Map<FormUserResponseDto>(formUserResponse);
                formUserResponseDto.SubMembers = model.SubMembersFormUserResponseDto;
                foreach (var response in formUserResponseDto.FieldsResponse)
                {
                    var field = await unitOfWork.Repository<Field>().GetByIdAsync(response.FieldId);
                    response.FieldName = field.Title;
                }
                var result = mapper.Map<ReturnFormUserResponseDto>(formUserResponseDto);
                if (form.RequestTypeId != 12)
                {
                    var fees = await CalculateFees(formUserResponseDto);
                    result.FeesDto = fees.Data;
                }
                return new ResponseHandler().Success(result);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<ReturnFormUserResponseDto>(ex.Message);
            }
        }
        private async Task<Response<List<FeesDto>>> CalculateFees(FormUserResponseDto formUserResponseDto)
        {
            try
            {
                var responseFees = await CalculateFeesForFormUserResponse(formUserResponseDto);
                var feesResult = new List<FeesDto>();
                feesResult.Add(responseFees.Data);
                if(formUserResponseDto.SubMembers is not null)
                    foreach (var response in formUserResponseDto.SubMembers)
                    {
                        var fees = await CalculateFeesForFormUserResponse(response);
                        feesResult.Add(fees.Data);
                    }

                return new ResponseHandler().Success(feesResult);
            }
            catch(Exception ex)
            {
                return new ResponseHandler().BadRequest<List<FeesDto>>(ex.Message);
            }
        }
        private async Task<Response<FeesDto>> CalculateFeesForFormUserResponse(FormUserResponseDto formUserResponseDto)
        {
            try
            {
                var formUserResponse = await unitOfWork.Repository<FormUserResponse>().GetByIdAsync(formUserResponseDto.Id.Value);
                var form = await formRepository.GetForm(formUserResponse.FormId);
                var fees = new FeesDto()
                {
                    RequestTypeId = form.RequestTypeId,
                    MemberTypeId = formUserResponse.MemberTypeId ?? 0
                };
                // For cancellation request set member type to Main Member
                if (form.RequestTypeId == 17)
                    fees.MemberTypeId = 5; 
                else if(form.RequestTypeId == 18)
                {
                    var submemberMembership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(formUserResponse.SubMemberId.Value);
                    fees.MemberTypeId = submemberMembership.MemberTypeId.Value;
                }
                var membershipRequest = await unitOfWork.Repository<MembershipRequest>().GetByIdAsync(formUserResponse.MembershipRequestId.Value);
                // To Get Form Fees updated from Configuration According to SubMember Member Type
                var packageMemberForm = await paakageMemberFormRepo.GetMemberFormByFormIdPackageIdmemberTypeId(membershipRequest.PackageId.Value, formUserResponse.FormId, fees.MemberTypeId);
                fees.FormFees = packageMemberForm.Fees;



                var requestType = await unitOfWork.Repository<RequestType>().GetByIdAsync(form.RequestTypeId);
                fees.RequestTypeName = requestType.Name;
                var membertype = await unitOfWork.Repository<MemberType>().GetByIdAsync(fees.MemberTypeId);
                fees.MemberTypeName = membertype.Name;
                if (form.RequestTypeId == 6)
                    fees.OperationalFees = 0;
                else
                {
                    //var membership = await unitOfWork.Repository<Membership>().GetByIdAsync(formUserResponse.MembershipId.Value);
                    if(form.RequestTypeId == 16 || form.RequestTypeId == 18)
                    {
                        fees.OperationalFees = await GetOperationalFees(form.RequestTypeId, membershipRequest.PackageId.Value, fees.MemberTypeId);
                    }
                    else
                        fees.OperationalFees = await GetOperationalFees(form.RequestTypeId, membershipRequest.PackageId.Value, fees.MemberTypeId);
                }
                return new ResponseHandler().Success(fees);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<FeesDto>(ex.Message);
            }
        }

        private async Task<Response<MembershipRequest>> CreateMembershipRequestForResponse(CreateMembershipRequestForResponseDto model)
        {
            try
            {
                var SuperMembershipRequest = new MembershipRequest();
                var CreateNewMembershipSubMembers = new List<MembershipRequest>();
                var FormUserResponse = await formUserResponseRepository.GetFormUserResponse(model.FormUserResponseId);
                // For New Membership Request
                if (model.Membership is null && model.Form.RequestTypeId == 6)
                {
                    SuperMembershipRequest = mapper.Map<MembershipRequest>(FormUserResponse);
                    if (model.SubMembersFormUserResponseId is not null)
                    {
                        foreach (var subMemberResponseId in model.SubMembersFormUserResponseId)
                        {
                            var SubFormUserResponse = await formUserResponseRepository.GetFormUserResponse(subMemberResponseId);
                            var SubMembershipRequest = await MappingSubMemberRequest(SubFormUserResponse, 16);
                            await unitOfWork.Repository<MembershipRequest>().Add(SubMembershipRequest);
                            SubMembershipRequest.PackageId = SuperMembershipRequest.PackageId;
                            await unitOfWork.Save();
                            CreateNewMembershipSubMembers.Add(SubMembershipRequest);
                            SubFormUserResponse.MembershipRequestId = SubMembershipRequest.Id;
                            await unitOfWork.Save();
                        }
                    }
                }
                // Like Addition , Removal , Separation
                if (model.Membership is not null)
                {
                    SuperMembershipRequest = mapper.Map<MembershipRequest>(model.Membership);
                    SuperMembershipRequest.SubMembers = new List<MembershipRequest>();
                }
                // mapping rest data of superMembership
                SuperMembershipRequest.Id = 0;
                SuperMembershipRequest.UserId = model.FormUserResponseDto.UserId;
                SuperMembershipRequest.RequestTypeId = model.Form.RequestTypeId;
                // this tanent Id should be dynamically later according to form tenant Id
                SuperMembershipRequest.TenantId = 1;
                // To set user Id in the Membership Request as Created by 
                SuperMembershipRequest.CreatedBy = model.FormUserResponseDto.UserId;

                // For Addition Request to map subMember data from response
                if (model.Form.RequestTypeId == 16)
                {
                    var SubMembershipRequest = await MappingSubMemberRequest(FormUserResponse, 16);
                    SubMembershipRequest.PackageId = SuperMembershipRequest.PackageId;

                    await unitOfWork.Repository<MembershipRequest>().Add(SubMembershipRequest);
                    await unitOfWork.Save();
                    CreateNewMembershipSubMembers.Add(SubMembershipRequest);
                    if (model.SubMembersFormUserResponseId is not null)
                    {
                        foreach (var subMemberResponseId in model.SubMembersFormUserResponseId)
                        {
                            var SubFormUserResponse = await formUserResponseRepository.GetFormUserResponse(subMemberResponseId);
                            var AddSubMembershipRequest = await MappingSubMemberRequest(SubFormUserResponse, 16);
                            AddSubMembershipRequest.PackageId = SuperMembershipRequest.PackageId;
                            await unitOfWork.Repository<MembershipRequest>().Add(AddSubMembershipRequest);
                            await unitOfWork.Save();
                            CreateNewMembershipSubMembers.Add(AddSubMembershipRequest);
                            SubFormUserResponse.MembershipRequestId = AddSubMembershipRequest.Id;
                            await unitOfWork.Save();
                        }
                    }
                }
                // For Removal Request OR Separation Request to set subMembershipId
                if ((model.Form.RequestTypeId == 18 || model.Form.RequestTypeId == 9) && model.FormUserResponseDto.SubMemberId.HasValue)
                {
                    // To set SubMember Membership Id  
                    FormUserResponse.SubMemberId = model.FormUserResponseDto.SubMemberId;
                    var SubMembershipRequest = mapper.Map<MembershipRequest>(model.SubMembership);
                    // mapping rest data of superMembership
                    SubMembershipRequest.Id = 0;
                    SubMembershipRequest.UserId = model.SubMembership.UserId;
                    SubMembershipRequest.RequestTypeId = model.Form.RequestTypeId;
                    // this tanent Id should be dynamically later according to form tenant Id
                    SubMembershipRequest.TenantId = 1;
                    // To set user Id in the Membership Request as Created by 
                    SubMembershipRequest.CreatedBy = model.FormUserResponseDto.UserId;

                    // Add submembership Request to the superMembership Request
                    SuperMembershipRequest.SubMembers.Add(SubMembershipRequest);
                }

                await unitOfWork.Repository<MembershipRequest>().Add(SuperMembershipRequest);
                await unitOfWork.Save();
                FormUserResponse.MembershipRequestId = SuperMembershipRequest.Id;
                var RequestStatusHistory = new List<RequestStatusHistory>();
                if (CreateNewMembershipSubMembers.Count > 0)
                    foreach (var item in CreateNewMembershipSubMembers)
                        item.SuperMembershipRequestId = SuperMembershipRequest.Id;

                if (model.Form.WorkflowSelection == "Direct Action")
                {
                    SuperMembershipRequest.Status = RequestStatus.RequestResolved;
                    RequestStatusHistory.Add(new RequestStatusHistory()
                    {
                        MembershipRequestId = SuperMembershipRequest.Id,
                        RequestStatusId = RequestStatus.RequestResolved,
                        CreationDate = DateTime.Now
                    });
                    // to set status of submember
                    if (SuperMembershipRequest.SubMembers?.Count > 0)
                        foreach (var subMember in SuperMembershipRequest.SubMembers)
                        {
                            subMember.Status = RequestStatus.RequestResolved;
                            RequestStatusHistory.Add(new RequestStatusHistory()
                            {
                                MembershipRequestId = subMember.Id,
                                RequestStatusId = RequestStatus.RequestResolved,
                                CreationDate = DateTime.Now
                            });
                        }

                    if (CreateNewMembershipSubMembers.Count > 0)
                        foreach (var item in CreateNewMembershipSubMembers)
                        {
                            item.Status = RequestStatus.RequestResolved;
                            RequestStatusHistory.Add(new RequestStatusHistory()
                            {
                                MembershipRequestId = item.Id,
                                RequestStatusId = RequestStatus.RequestResolved,
                                CreationDate = DateTime.Now
                            });
                        }
                            
                    // Excute Logic For Request Type

                    var data = new ExecuteLogicForRequestTypeDto()
                    {
                        FormUserResponseId = FormUserResponse.Id,
                        RequestTypeId = model.Form.RequestTypeId
                    };
                    data.SubMembershipRequestId = SuperMembershipRequest.SubMembers?.Count > 0 ? SuperMembershipRequest.SubMembers[0].Id : null;
                    data.SubMembershipRequestsId = CreateNewMembershipSubMembers.Count > 0 ? CreateNewMembershipSubMembers.Select(subReq => subReq.Id).ToList() : null;
                    await ExecuteLogicForRequestType(data);
                }
                else if (model.Form.WorkflowSelection == "Approval Workflow")
                {
                    SuperMembershipRequest.Status = RequestStatus.Pending;
                    RequestStatusHistory.Add(new RequestStatusHistory()
                    {
                        MembershipRequestId = SuperMembershipRequest.Id,
                        RequestStatusId = RequestStatus.Pending,
                        CreationDate = DateTime.Now
                    });
                    // to set status of submember
                    if (SuperMembershipRequest.SubMembers?.Count > 0)
                        foreach (var subMember in SuperMembershipRequest.SubMembers)
                        {
                            subMember.Status = RequestStatus.Pending;
                            RequestStatusHistory.Add(new RequestStatusHistory()
                            {
                                MembershipRequestId = subMember.Id,
                                RequestStatusId = RequestStatus.Pending,
                                CreationDate = DateTime.Now
                            });
                        }

                    if (CreateNewMembershipSubMembers.Count > 0)
                        foreach (var item in CreateNewMembershipSubMembers)
                        {
                            item.Status = RequestStatus.Pending;
                            RequestStatusHistory.Add(new RequestStatusHistory()
                            {
                                MembershipRequestId = item.Id,
                                RequestStatusId = RequestStatus.Pending,
                                CreationDate = DateTime.Now
                            });
                        }
                }
                unitOfWork.Repository<RequestStatusHistory>().AddRange(RequestStatusHistory);
                await unitOfWork.Save();
                return new ResponseHandler().Success(SuperMembershipRequest);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequest>(ex.Message);
            }
        }
        private async Task<MembershipRequest> MappingSubMemberRequest(FormUserResponse FormUserResponse, int RequestTypeId)
        {
            var SubMembershipRequest = mapper.Map<MembershipRequest>(FormUserResponse);
            SubMembershipRequest.Id = 0;
            SubMembershipRequest.UserId = FormUserResponse.UserId;
            SubMembershipRequest.RequestTypeId = RequestTypeId; // set form Request Type
            // this tanent Id should be dynamically later according to form tenant Id
            SubMembershipRequest.TenantId = 1;
            // To set user Id in the Membership Request as Created by 
            SubMembershipRequest.CreatedBy = FormUserResponse.UserId;
            return SubMembershipRequest;
        }
        public async Task<Response<string>> ExecuteLogicForRequestType(ExecuteLogicForRequestTypeDto model)
        {
            try
            {
                var FormUserResponse = await formUserResponseRepository.GetFormUserResponse(model.FormUserResponseId);
                switch (model.RequestTypeId)
                {
                    case 6:
                        var membershipId = await ExecuteNewMembershipLogic(FormUserResponse.MembershipRequestId.Value);
                        if (model.SubMembershipRequestsId?.Count > 0)
                            foreach (var MembershipRequestId in model.SubMembershipRequestsId)
                                await ExecuteAdditionLogic(MembershipRequestId, membershipId.Data);
                        break;
                    case 9:
                        await ExecuteSeparationLogic(FormUserResponse.SubMemberId.Value);
                        break;
                    case 12:
                        var freezData = await GetFreezDuration(FormUserResponse);
                        freezData.Data.MembershipId = FormUserResponse.MembershipId.Value;
                        await ExecuteFreezLogic(freezData.Data, FormUserResponse);
                        break;
                    case 16:
                        if (model.SubMembershipRequestsId?.Count > 0)
                            foreach (var MembershipRequestId in model.SubMembershipRequestsId)
                                await ExecuteAdditionLogic(MembershipRequestId);
                        break;
                    case 17:
                        await ExecuteCancellation(FormUserResponse.MembershipId.Value, FormUserResponse);
                        break;
                    case 18:
                        await ExecuteRemovalLogic(FormUserResponse.SubMemberId.Value, FormUserResponse);
                        break;
                }
                return new ResponseHandler().Success("");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>(ex.Message);
            }
        }
        private async Task<Response<Data.Membership.Entities.Membership>> ExecuteCancellation(int MembershipId, FormUserResponse formUserResponse)
        {
            try
            {
                FieldResponse preferredCancellationDate = formUserResponse.FieldsResponse.FirstOrDefault(response => response.FieldId == 70);
                var MembershipRequestBackgroundJob = new MembershipRequestBackgroundJobs()
                {
                    MembershipRequestId = formUserResponse.MembershipRequestId.Value
                };
                if (preferredCancellationDate is not null)
                {
                    var cancellationDate = DateTime.Parse(preferredCancellationDate.Response);
                    MembershipRequestBackgroundJob.BackgroundJobId = backgroundJobClient.Schedule(() => CancelMembership(MembershipId), cancellationDate);
                }
                else
                    MembershipRequestBackgroundJob.BackgroundJobId = backgroundJobClient.Schedule(() => CancelMembership(MembershipId), DateTime.Now);
                await unitOfWork.Repository<MembershipRequestBackgroundJobs>().Add(MembershipRequestBackgroundJob);
                await unitOfWork.Save();
                return new ResponseHandler().Success(new Data.Membership.Entities.Membership());

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<Data.Membership.Entities.Membership>(ex.Message);
            }
        }
        public async Task CancelMembership(int MembershipId)
        {
            var membership = await membershipRepository.GetMembershipWithSubMembers(MembershipId);
            membership.MembershipStatus = MembershipStatus.Cancelled;
            await DeleteMembershipJobs(MembershipId);
            foreach (var subMember in membership.SubMembers)
            {
                subMember.MembershipStatus = MembershipStatus.Cancelled;
                await DeleteMembershipJobs(subMember.Id);
            }
            await unitOfWork.Save();
        }
        private async Task DeleteMembershipJobs(int MembershipId)
        {
            var MembershipRequests = await membershipRequestRepository.GetResolvedRequestsOfMembership(MembershipId);
            foreach (var request in MembershipRequests)
            {
                var requestBackgroundJobs = await membershipRequestRepository.GetMembershipRequestJobs(request.Id);
                foreach (var job in requestBackgroundJobs)
                {
                    BackgroundJob.Delete(job.BackgroundJobId);
                }
            }
        }
        public async Task<Response<GetFreezCardDto>> GetFreezCard(int MembershipId)
        {
            try
            {
                var result = new GetFreezCardDto();
                var totalFreezDaysUsed = 0;
                var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(MembershipId);
                var packageFreezConfig = await freezeConfigurationService.GetByPackageId(membership.PackageId.Value);
                var freezRequests = await membershipRequestRepository.GetRequestsOfMembershipByType(12, MembershipId);
                var freezFormResponses = new List<FormUserResponse>();
                foreach(var request in freezRequests)
                {
                    var response = await formUserResponseRepository.GetResponseByRequestId(request.Id);
                    freezFormResponses.Add(response);
                    var freezData = await GetFreezDuration(response);
                    totalFreezDaysUsed += freezData.Data.FreezDuration;
                }
                result.TotalFreezDaysUsed = totalFreezDaysUsed;
                if (packageFreezConfig.Data is not null)
                {
                    result.FreezLimit = packageFreezConfig.Data.FreezeLimit;
                    result.PricePerAdditionalDays = packageFreezConfig.Data.PricePerAdditionalDay;
                    result.RemainingFreezDays = result.FreezLimit - totalFreezDaysUsed;
                    result.CanExceedLimit = packageFreezConfig.Data.CanExceed;
                }
                result.PreviousFreezes = new List<PreviousFreezRequest>();
                if (freezRequests.Count > 0)
                {
                    for(var i = freezRequests.Count-1; i>=freezRequests.Count-2 && i >= 0; i--)
                    {
                        var response = freezFormResponses[i];
                        var freezData = await GetFreezDuration(response);
                        var PreviousFreezRequest = new PreviousFreezRequest();
                        PreviousFreezRequest.StartDate = freezData.Data.StartDate;
                        PreviousFreezRequest.EndDate = freezData.Data.EndDate;
                        PreviousFreezRequest.FreezDuration = freezData.Data.FreezDuration;
                        result.PreviousFreezes.Add(PreviousFreezRequest);
                    }
                }

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetFreezCardDto>(ex.Message);
            }
        }
        public async Task<Response<FreezAdditionalPayment>> CheckExceedWithAdditionalPayment(FormUserResponseDto formUserResponseDto)
        {
            try
            {
                var result = new FreezAdditionalPayment();
                var freezCard = await GetFreezCard(formUserResponseDto.MembershipId.Value);
                var formUserResponse = mapper.Map<FormUserResponse>(formUserResponseDto);
                var duration = await GetFreezDuration(formUserResponse);
                var freezDuration = duration.Data.FreezDuration;
                if (freezDuration > freezCard.Data.FreezLimit)
                {
                    result.Message = $"You have selected {freezDuration} days, but you only have {freezCard.Data.RemainingFreezDays} freeze days remaining. The price for 1 additional day is {freezCard.Data.PricePerAdditionalDays} EGP. Would you like to proceed with the additional charge?";
                    result.HasAdditionalPayment = true;
                    result.SelectedFreezePeriod = freezDuration;
                    result.IncludedFreezeDays = freezCard.Data.RemainingFreezDays;
                    result.AdditionalDays = result.SelectedFreezePeriod - result.IncludedFreezeDays;
                    result.CostForAdditionalDay = freezCard.Data.PricePerAdditionalDays;
                    result.TotalCost = result.AdditionalDays * result.CostForAdditionalDay;
                    return new ResponseHandler().Success(result);
                }
                else
                {
                    result.HasAdditionalPayment = false;
                    return new ResponseHandler().Success(result);
                }
            }
            catch(Exception ex)
            {
                return new ResponseHandler().BadRequest<FreezAdditionalPayment>(ex.Message);
            }
        }

        public async Task<Response<bool>> CheckCanSelectPackage(int packageId, string userId)
        {
            try
            {
                var membership = await membershipRepository.GetUserMembershipByPackageId(userId, packageId);
                if (membership is null)
                    return new ResponseHandler().Success(true);
                return new ResponseHandler().BadRequest<bool>("You are already subscribed to this package");
            }
            catch(Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>(ex.Message);
            }
        }

        private async Task<Response<FreezData>> GetFreezDuration(FormUserResponse formUserResponse)
        {
            try
            {
                int duration = 0;
                FieldResponse durationResponse = formUserResponse.FieldsResponse.FirstOrDefault(response => response.FieldId == 24);
                FieldResponse startDateResponse = formUserResponse.FieldsResponse.FirstOrDefault(response => response.FieldId == 22);
                FieldResponse endDateResponse = formUserResponse.FieldsResponse.FirstOrDefault(response => response.FieldId == 23);
                DateTime startDate = DateTime.Parse(startDateResponse.Response);
                DateTime? endDate = null;
                // check whether response have Duration or not
                if (durationResponse is not null)
                {
                    duration = int.Parse(durationResponse.Response);
                    endDate = startDate.AddDays(duration);
                }
                else if (endDateResponse is not null)
                {
                    endDate = DateTime.Parse(endDateResponse.Response);
                    duration = (endDate.Value - startDate).Days;
                }
                var freezData = new FreezData()
                {
                    FreezDuration = duration,
                    StartDate = startDate
                };
                if (endDate is not null)
                    freezData.EndDate = endDate.Value;
                return new ResponseHandler().Success(freezData);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<FreezData>(ex.Message);
            }
        }
        private async Task<Response<Data.Membership.Entities.Membership>> ExecuteFreezLogic(FreezData freezData, FormUserResponse formUserResponse)
        {
            try
            {
                var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(freezData.MembershipId);
                membership.EndDate = membership.EndDate.Value.AddDays(freezData.FreezDuration);
                await unitOfWork.Save();
                var FreezBackgroundJob = new MembershipRequestBackgroundJobs()
                {
                    MembershipRequestId = formUserResponse.MembershipRequestId.Value
                };
                FreezBackgroundJob.BackgroundJobId = backgroundJobClient.Schedule(() => ChangeMembershipStatus(membership.Id, MembershipStatus.Freezed), freezData.StartDate);
                await unitOfWork.Repository<MembershipRequestBackgroundJobs>().Add(FreezBackgroundJob);
                await unitOfWork.Save();
                var ActiveBackgroundJob = new MembershipRequestBackgroundJobs()
                {
                    MembershipRequestId = formUserResponse.MembershipRequestId.Value
                };
                ActiveBackgroundJob.BackgroundJobId = backgroundJobClient.Schedule(() => ChangeMembershipStatus(membership.Id, MembershipStatus.Active), freezData.EndDate);
                await unitOfWork.Repository<MembershipRequestBackgroundJobs>().Add(ActiveBackgroundJob);
                await unitOfWork.Save();
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
            await unitOfWork.Save();
        }
        public async Task ChangeMembershipStatusInRemovalRequest(int membershipId, MembershipStatus status)
        {
            var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(membershipId);
            membership.MembershipStatus = status;
           // membership.IsDeleted = true;
            await unitOfWork.Save();
            await DeleteMembershipJobs(membershipId);
        }
        // New Commemnt
        private async Task<Response<string>> ExecuteRemovalLogic(int SubMembershipId, FormUserResponse formUserResponse)
        {
            try
            {
                FieldResponse effectiveDate = formUserResponse.FieldsResponse.FirstOrDefault(response => response.FieldId == 49);
                var RemovalBackgroundJob = new MembershipRequestBackgroundJobs()
                {
                    MembershipRequestId = formUserResponse.MembershipRequestId.Value
                };
                if (effectiveDate is null)
                    backgroundJobClient.Schedule(() => ChangeMembershipStatusInRemovalRequest(SubMembershipId, MembershipStatus.Removed), DateTime.Now);
                else
                {
                    DateTime removalEffectiveDate = DateTime.Parse(effectiveDate.Response);
                    RemovalBackgroundJob.BackgroundJobId = backgroundJobClient.Schedule(() => ChangeMembershipStatusInRemovalRequest(SubMembershipId, MembershipStatus.Removed), removalEffectiveDate);
                    await unitOfWork.Repository<MembershipRequestBackgroundJobs>().Add(RemovalBackgroundJob);
                    await unitOfWork.Save();
                }
                return new ResponseHandler().Success("");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>(ex.Message);
            }
        }
        private async Task<Response<string>> ExecuteSeparationLogic(int SubMembershipId)
        {
            try
            {
                var Submembership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(SubMembershipId);
                Submembership.ParentMembershipId = null;
                Submembership.MemberShipId = GenerateMembershipId();
                await unitOfWork.Save();
                return new ResponseHandler().Success("");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>(ex.Message);
            }
        }
        private async Task<Response<int>> ExecuteNewMembershipLogic(int MembershipRequestId)
        {
            try
            {
                var membershipRequest = await unitOfWork.Repository<MembershipRequest>().GetByIdAsync(MembershipRequestId);
                var membership = mapper.Map<Data.Membership.Entities.Membership>(membershipRequest);
                membership.Id = 0;
                membership.MembershipStatus = MembershipStatus.Active;
                membership.StartDate = DateTime.Now;
                membership.MemberShipId = GenerateMembershipId();
                var result = await ComputeEndDateOfMembershipByPackage(membershipRequest.PackageId.Value, membership.StartDate.Value);
                membership.EndDate = result.Data;
                membership.SubMembers = new List<Data.Membership.Entities.Membership>();
                await unitOfWork.Repository<Data.Membership.Entities.Membership>().Add(membership);
                await unitOfWork.Save();
                membershipRequest.MembershipId = membership.Id;
                await unitOfWork.Save();
                return new ResponseHandler().Success(membership.Id);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<int>(ex.Message);
            }
        }
        private async Task<Response<string>> ExecuteAdditionLogic(int SubMembershipRequestId, int SuperMembershipId = 0)
        {
            try
            {
                var subMembershipRequest = await unitOfWork.Repository<MembershipRequest>().GetByIdAsync(SubMembershipRequestId);
                var parentId = SuperMembershipId ==0 ? subMembershipRequest.MembershipId.Value : SuperMembershipId;
                var parentMembership = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetByIdAsync(parentId);

                var subMembership = mapper.Map<Data.Membership.Entities.Membership>(subMembershipRequest);
                
                subMembership.MemberShipId = parentMembership.MemberShipId;
                subMembership.Id = 0;
                subMembership.ParentMembershipId = SuperMembershipId == 0 ? subMembershipRequest.MembershipId : SuperMembershipId;
                subMembership.MembershipStatus = MembershipStatus.Active;
                subMembership.StartDate = DateTime.Now;
                var result = await ComputeEndDateOfMembershipByPackage(subMembershipRequest.PackageId.Value, subMembership.StartDate.Value);
                subMembership.EndDate = result.Data;
                await unitOfWork.Repository<Data.Membership.Entities.Membership>().Add(subMembership);
                await unitOfWork.Save();
                subMembershipRequest.MembershipId = subMembership.Id;
                await unitOfWork.Save();
                return new ResponseHandler().Success("");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>(ex.Message);
            }
        }
        public async Task<Response<DateTime>> ComputeEndDateOfMembershipByPackage(int packageId, DateTime StartDate)
        {
            try
            {
                var client = new HttpClient();
                var url = $"{configuration["ApiSettings:Gateway"]}/EMembership/1/Package/GetPackageByIdAsync/{packageId}";
                var token = httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(url);
                var responseAsString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Response<PackageDataDto>>(responseAsString);
                var endDate = StartDate.AddMonths(result.Data.Duration.Value);
                return new ResponseHandler().Success(endDate);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<DateTime>(ex.Message);
            }
        }

        public async Task<Response<List<FormDto>>> GetFormsByMemberTypeAndRequestType(int memberTypeId, int requestTypeId)
        {
            try
            {
                var forms = await formRepository.GetFormsByTypeAndMemberTypeId(memberTypeId, requestTypeId);
                var formDtos = mapper.Map<List<FormDto>>(forms);

                if (forms is null)
                    return new ResponseHandler().BadRequest<List<FormDto>>("There is no forms by this member");

                return new ResponseHandler().Success(formDtos);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<FormDto>>(ex.Message);
            }
        }

        public async Task<Response<string>> UploadFile(IFormFile file)
        {
            try
            {
                var url = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", file);
                return new ResponseHandler().Success(url);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>(ex.Message);
            }
        }

        public async Task<Response<Dictionary<string, List<FormDto>>>> GetFormsForEachSubMemberAsync(int packageId, int requestTypeId)
        {
            try
            {
                // Step 1: Get the list of MemberTypeIds (SubMember IDs) for the given package
                var memberTypeIds = await packageRequestAuthorizationRepository.GetPackageMemberTypeIdsAsync(packageId);

                // Step 2: Initialize a dictionary to store forms by MemberType name
                var formsByMemberTypeName = new Dictionary<string, List<FormDto>>();

                // Step 3: Iterate through each MemberTypeId and fetch forms
                foreach (var memberTypeId in memberTypeIds)
                {
                    // Retrieve the MemberType name by ID
                    var memberType = await unitOfWork.Repository<MemberType>().GetByIdAsync(memberTypeId);

                    if (string.IsNullOrEmpty(memberType?.Name))
                    {
                        continue; // Skip if the MemberTypeName is null or empty
                    }

                    // Retrieve the forms for this specific MemberTypeId and RequestTypeId
                    var forms = await formRepository.GetFormsByTypeAndMemberTypeId(memberTypeId, requestTypeId);
                    var formDtos = mapper.Map<List<FormDto>>(forms);

                    // Store the forms in the dictionary with MemberTypeName as the key
                    formsByMemberTypeName[memberType.Name] = formDtos;
                }

                // Step 4: Return a success response with the dictionary as data
                return new ResponseHandler().Success(formsByMemberTypeName);
            }
            catch (Exception ex)
            {
                // Step 5: Handle any exceptions and return a BadRequest response
                return new ResponseHandler().BadRequest<Dictionary<string, List<FormDto>>>(ex.Message);
            }
        }
        public async Task<Response<FormDto>> GetFormsForSubMemberAsync(int packageId,int requestTypeId,int? memberTypeId = null)
        {
            try
            {
                int resolvedMemberTypeId;

                // New Membership → Main
                if (requestTypeId == 6|| requestTypeId == 17)
                {
                    resolvedMemberTypeId = 5; // Main
                }

              

                // Other requests requiring selection
                else
                {
                    if (!memberTypeId.HasValue)
                    {
                        return new ResponseHandler()
                            .BadRequest<FormDto>("Member type is required");
                    }

                    resolvedMemberTypeId = memberTypeId.Value;
                }

                // Build specification
                var spec = new GeneralSpecifications<PaakageMemberForm>(x =>
                    x.PackageId == packageId &&
                    x.MemberTypeId == resolvedMemberTypeId &&
                    x.Form.RequestTypeId == requestTypeId
                );

                // Includes
                spec.AddInclude(q => q.Include(x => x.MemberType));

                spec.AddInclude(q => q.Include(x => x.Form)
                    .ThenInclude(f => f.FormFields)
                    .ThenInclude(ff => ff.Field)
                    .ThenInclude(f => f.FieldType));

                spec.AddInclude(q => q.Include(x => x.Form)
                    .ThenInclude(f => f.FormFields)
                    .ThenInclude(ff => ff.Field)
                    .ThenInclude(f => f.DropdownValues));

                var packageForm = await unitOfWork
                    .Repository<PaakageMemberForm>()
                    .GetWithSpecifications(spec);

                if (packageForm == null)
                {
                    return new ResponseHandler()
                        .Success(new FormDto());
                }

                var result = mapper.Map<FormDto>(packageForm.Form);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler()
                    .BadRequest<FormDto>(ex.Message);
            }
        }
        public async Task<Response<List<ConfigurationResponseDto>>> GetPackageDetailsByRequestTypeAndPackageID(int requestTypeId, int packageID)
        {
            try
            {
                var result = await configurationRepository.GetPackageDetailsByRequestTypeAndPackageID(requestTypeId, packageID);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<ConfigurationResponseDto>>(ex.Message);
            }
        }

        public async Task<Response<IReadOnlyList<NewRegistrationFormToReturnDto>>> GetNewRegistrationFormsByRequestTypeId()
        {
            try
            {

                const int newMembershipRequestsID = 6;

                var requestType = await requestTypeRepository.GetByIdAsync(newMembershipRequestsID);

                if (requestType is null)
                    return new ResponseHandler().NotFound<IReadOnlyList<NewRegistrationFormToReturnDto>>("New Membership Request Id was not found");

                var newRegistartionForms = await formRepository.GetNewRegistrationFormsByRequestIdAsync(requestType.Id);

                var users = newRegistartionForms.Select(f => f.CreatedBy).ToList();

                if (users.Count > 0)
                {
                    var httpClient = new HttpClient();

                    var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

                    token = token.Replace("Bearer ", "");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                    var url = $"{configuration["ApiSettings:Gateway"]}{configuration["MySelfOldUrl:GetUsers"]}";

                    var usersData = await httpClient.PostAsJsonDynamic(url, users);

                    var userDataAsList = ((IEnumerable<dynamic>)usersData).ToList();

                    var isNotValid = userDataAsList.Exists(p => p.id == null);

                    if (isNotValid)
                        return new ResponseHandler().NotFound<IReadOnlyList<NewRegistrationFormToReturnDto>>("Invalid admins Id");

                    for (int i = 0; i < newRegistartionForms.Count; i++)
                    {

                        var currentUser = userDataAsList.FirstOrDefault(u => (string)u.id == newRegistartionForms[i].CreatedBy);

                        newRegistartionForms[i].CreatedBy = (string)(currentUser?.userName) ?? "Unkown";
                    }
                    var y = 6;

                    var mappedNewRegistartionFormsToReturn = mapper.Map<IReadOnlyList<Form>, IReadOnlyList<NewRegistrationFormToReturnDto>>(newRegistartionForms);

                    return new ResponseHandler().Success(mappedNewRegistartionFormsToReturn);

                }
                var x = 6;

                var mappedNewRegistartionForms = mapper.Map<IReadOnlyList<Form>, IReadOnlyList<NewRegistrationFormToReturnDto>>(newRegistartionForms);

                return new ResponseHandler().Success(mappedNewRegistartionForms);

            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(GetNewRegistrationFormsByRequestTypeId)}: {ex.Message}");
                return new ResponseHandler().BadRequest<IReadOnlyList<NewRegistrationFormToReturnDto>>("something went Wrong!!");
            }

        }

        public async Task<Response<NewRegistrationFormToReturnDto>> GetNewRegistrationFormsById(int Id)
        {
            try
            {
                var newRegistartionForms = await formRepository.GetNewRegistrationFormsByIdAsync(Id);

                if (newRegistartionForms is null)
                    return new ResponseHandler().NotFound<NewRegistrationFormToReturnDto>("Invalid new registration forms Id");


                if (!string.IsNullOrEmpty(newRegistartionForms.CreatedBy))
                {
                    var user = new List<string> { newRegistartionForms.CreatedBy };

                    var httpClient = new HttpClient();

                    var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

                    token = token.Replace("Bearer ", "");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                    var url = $"{configuration["ApiSettings:Gateway"]}{configuration["MySelfOldUrl:GetUsers"]}";

                    var usersData = await httpClient.PostAsJsonDynamic(url, user);

                    var userDataAsList = ((IEnumerable<dynamic>)usersData).ToList();

                    var isNotValid = userDataAsList.Exists(p => p.id.ToString() == null);

                    if (isNotValid)
                        return new ResponseHandler().NotFound<NewRegistrationFormToReturnDto>("Invalid admins Id");



                    var currentUser = userDataAsList.FirstOrDefault(u => (string)u.id == newRegistartionForms.CreatedBy) ?? "Unkown";

                    newRegistartionForms.CreatedBy = (string)currentUser?.userName ?? "Unkown";


                    var mappedNewRegistartionFormsToReturn = mapper.Map<Form, NewRegistrationFormToReturnDto>(newRegistartionForms);

                    return new ResponseHandler().Success(mappedNewRegistartionFormsToReturn);

                }

                var mappedNewRegistartionForms = mapper.Map<Form, NewRegistrationFormToReturnDto>(newRegistartionForms);

                return new ResponseHandler().Success(mappedNewRegistartionForms);

            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<NewRegistrationFormToReturnDto>(ex.Message);
            }

        }
        public async Task<Response<CreateUpdateFormDto>> CreateUpdateForm(CreateUpdateFormDto formDto)
        {
            try
            {
                if (!formDto.TenantId.HasValue)
                {
                    Log.Error("TenantId is missing in the CreateUpdateFormDto");
                    return new ResponseHandler().BadRequest<CreateUpdateFormDto>("TenantId is missing");
                }

                var validationResponse = await ValidateFormAsync(formDto);

                if (validationResponse != null)
                    return validationResponse;

                Form form;

                if (IsNewForm(formDto.Id))
                {
                    var result = await CreateForm(new Form(), formDto);
                    form = result.Data;
                }
                else
                {
                    var result = await UpdateForm(formDto);
                    form = result.Data;
                }

                if (form == null)
                    return new ResponseHandler().BadRequest<CreateUpdateFormDto>("Couldn't Create Or Update The Form");

                formDto = mapper.Map<CreateUpdateFormDto>(form);


                return new ResponseHandler().Success(formDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<CreateUpdateFormDto>(ex.Message);
            }
        }

        private async Task<Response<Form>> CreateForm(Form form,CreateUpdateFormDto formDto)
        {
            try
            {

                MappingForm(form, formDto);
                await unitOfWork.Repository<Form>().Add(form);

                await HandleCustomFormFields(form, formDto);

                await unitOfWork.Save();

                return new ResponseHandler().Success(form);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<Form>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task<Response<Form>> UpdateForm(CreateUpdateFormDto formDto)
        {
            try
            {
               
                

                var form = await formRepository.GetForm((int)formDto.Id!);
                if (form == null) 
                    return new ResponseHandler().BadRequest<Form>("Form not found");

                MappingForm(form, formDto);

                unitOfWork.Repository<Form>().Update(form);
                await unitOfWork.Save();

                return new ResponseHandler().Success(form);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<Form>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private void MappingForm(Form form, CreateUpdateFormDto formDto)
        {
            form.Fees = formDto.Fees;
            form.Title = formDto.Title;
            form.Alias = formDto.Alias;
            form.WorkflowSelection = formDto.WorkflowSelection;
            form.FormFields = mapper.Map<List<FormFields>>(formDto.FieldsDto);
            form.RequestTypeId = formDto.RequestTypeId;
            form.CreatedBy = formDto.CreatedBy;
            form.TenantId = formDto.TenantId.Value;
            form.IsDraft = formDto.IsDraft;
            form.ModificationDate = DateTime.UtcNow;
            mappingMemberTypes(form, formDto);
        }


        private void mappingMemberTypes(Form form, CreateUpdateFormDto formDto)
        {
            // Add Form Member Types
            var formMemberTypes = new List<FormMemberType>();
            if (formDto.MemberTypesId != null)
                foreach (var id in formDto.MemberTypesId)
                {
                    var formMemberType = new FormMemberType
                    {
                        FormId = form.Id,
                        MemberTypeId = id
                    };
                    formMemberTypes.Add(formMemberType);
                }

            form.MemberTypes = formMemberTypes;

        }


        private async Task<Response<string>> HandleCustomFormFields(Form form, CreateUpdateFormDto formDto)
        {
            try
            {
                if (formDto.CustomFormFieldsDtos is null || !formDto.CustomFormFieldsDtos.Any())
                {
                    Log.Information("No custom form fields to handle.");
                    return new ResponseHandler().Success("No custom form fields to handle.");
                }

                var startIndex = form.FormFields?.Count ?? 0;

                var customFields = formDto.CustomFormFieldsDtos.Select((dto, i) => new Field
                {
                    Title = dto.Title,
                    TitleAr = dto.TitleAr,
                    FieldTypeId = dto.FieldTypeId,
                    UnitTypeId = dto.UnitTypeId,
                    MapField = dto.MapField,
                    DropdownValues = dto.DropdownValues?
                        .Select(v => new DropdownValue { Value = v })
                        .ToList(),
                    FormFields = new List<FormFields>
                    {
                        new FormFields
                        {
                            Form = form,
                            IsRequired = dto.IsRequired,
                            IsHidden = dto.IsAdminFilled,
                            Index = startIndex + i
                        }
                    }
                }).ToList();

                Log.Information($"Mapped {customFields.Count} custom fields from DTO.");

                unitOfWork.Repository<Field>().AddRange(customFields);

                Log.Information($"Saved {customFields.Count} custom fields and linked to Form ID {form.Id}.");

                return new ResponseHandler().Success("Custom fields created and linked successfully.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error in {nameof(HandleCustomFormFields)}: {ex.Message}");
                return new ResponseHandler().BadRequest<string>("Something went wrong!");
            }
        }


        private async Task<Response<CreateUpdateFormDto>> ValidateFormAsync(CreateUpdateFormDto formDto)
        {
            // Validate workflow selection
            if (!formDto.IsDraft)
            {
                if (formDto.WorkflowSelection != "Approval Workflow" && formDto.WorkflowSelection != "Direct Action")
                    return new ResponseHandler().BadRequest<CreateUpdateFormDto>("The value of WorkflowSelection must be 'Approval Workflow' or 'Direct Action'");

            }
            var requestType = await requestTypeRepository.GetRequestTypeWithFields(formDto.RequestTypeId);

            // Validation for core and Mandatory
            foreach (var requestTypeField in requestType.Fields)
            {
                if (requestTypeField.FieldCategoryId == 1 )
                {
                    var formField = formDto.FieldsDto.FirstOrDefault(f => f.FieldId == requestTypeField.FieldId);
                    if (formField is null)
                        return new ResponseHandler().BadRequest<CreateUpdateFormDto>("you have to select all Core  fields");
                }
            }


            // Validate request type and its fields
            if (requestType == null)
                return new ResponseHandler().BadRequest<CreateUpdateFormDto>("There is no Request Type with this ID");

            var requestTypeFieldIds = requestType.Fields.Select(f => f.FieldId).ToList();
            if (!formDto.FieldsDto.All(field => requestTypeFieldIds.Contains(field.FieldId)))
                return new ResponseHandler().BadRequest<CreateUpdateFormDto>("One or more fields have an invalid ID");

            // Validate member types if present
            if (formDto.MemberTypesId != null)
            {
                var memberTypes = await unitOfWork.Repository<MemberType>().GetAllAsync();
                var memberTypeIds = memberTypes.Select(type => type.Id).ToList();

                foreach (var id in formDto.MemberTypesId)
                {
                    if (!memberTypeIds.Contains(id))
                        return new ResponseHandler().BadRequest<CreateUpdateFormDto>($"There is no Member Type with ID = {id}");

                    if (id != 5 && formDto.RequestTypeId == 17)
                        return new ResponseHandler().BadRequest<CreateUpdateFormDto>("You can only add Main Member type in cancellation");

                    if (id != 5 && formDto.RequestTypeId == 6)
                        return new ResponseHandler().BadRequest<CreateUpdateFormDto>("You can only add Main Member type in Create New Membership Form");
                }
            }


            //validate custom fields if present

            if(formDto.CustomFormFieldsDtos is not null && formDto.CustomFormFieldsDtos.Any())
            {
                var customFieldTypeIds = formDto.CustomFormFieldsDtos
                .Select(f => f.FieldTypeId)
                .Distinct()
                .ToList();

                var fieldTypes = await unitOfWork.Repository<FieldType>().FindAllByCriteriaAsync(ft => customFieldTypeIds.Contains(ft.Id));

                if(fieldTypes.Count != customFieldTypeIds.Count)
                    return new ResponseHandler().BadRequest<CreateUpdateFormDto>("One or more custom fields have an invalid FieldTypeId");


                var customUnitTypeIds = formDto.CustomFormFieldsDtos.Where(f => f.UnitTypeId.HasValue).Select(f => f.UnitTypeId.Value).ToList();
                if (customUnitTypeIds.Count > 0)
                {
                    var unitTypes = await unitOfWork.Repository<UnitType>().FindAllByCriteriaAsync(ut => customUnitTypeIds.Contains(ut.Id));
                    if (unitTypes.Count != customUnitTypeIds.Count)
                        return new ResponseHandler().BadRequest<CreateUpdateFormDto>("One or more custom fields have an invalid UnitTypeId");
                }
            }

            // If all validations pass, return null to indicate success
            return null;
        }

        private bool IsNewForm(int? id)
        {
            return id is null || id == 0 ;
        }

        private string GenerateMembershipId()
        {
           
            Guid guid = Guid.NewGuid();

            
            int uniqueNumber = Math.Abs(BitConverter.ToInt32(guid.ToByteArray(), 0));

       
            string timestamp = DateTime.Now.Ticks.ToString();

       
            var uniqueId = uniqueNumber.ToString() + timestamp;

            return uniqueId;
        }

        public async Task<Response<ReturnDraftFormDto>> GetDraftForm(int id, int? packageId)
        {

            try
            {
                var form = await formRepository.GetForm(id);
                if (form is null)
                    return new ResponseHandler().BadRequest<ReturnDraftFormDto>("There is no form with this Id");
                var formDto = GetReturnDraft(form);

                formDto.CategoriesFields = await GetFieldsGroupedByCategory(form);
                formDto.MemberTypes = await GetDraftedMemberType(form);

                return new ResponseHandler().Success(formDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<ReturnDraftFormDto>(ex.Message);
            }
        }


        private ReturnDraftFormDto GetReturnDraft(Form form) 
        {
            return new ReturnDraftFormDto()
            {
                Id = form.Id,
                CreationDate = form.CreationDate,
                Alias = form.Alias,
                CreatedBy = form.CreatedBy,
                Fees = form.Fees,
                IsDraft = form.IsDraft,
                Title = form.Title,
                WorkflowSelection = form.WorkflowSelection,
                RequestTypeName = form.RequestType.Name,
                RequestTypeId = form.RequestTypeId,

            };
            
        }

        private async Task<List<DraftMemberTypeDto>> GetDraftedMemberType(Form form)
        {
            try
            {
                var memberTypes = await memberTypeRepo.GetMemberTypes(null, null);
                var types = mapper.Map<List<DraftMemberTypeDto>>(memberTypes);
                if (form.MemberTypes != null || form.MemberTypes?.Count > 0)
                {
                    foreach (var memberType in types)
                    {
                        if (form.MemberTypes.Any(m => m.MemberTypeId == memberType.Id))
                            memberType.WasSelected = true;


                    }
                }

                return types;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task<List<DraftFieldsGroupedByCategory>> GetFieldsGroupedByCategory(Form form)
        {
            try
            {
                var requestType = await requestTypeRepository.GetRequestTypeWithFields(form.RequestTypeId);

                var result = new List<DraftFieldDto>();
                foreach (var item in requestType.Fields)
                {
                    var field = await fieldRepository.GetFieldWithDropdownValues(item.FieldId);
                    var fieldDto = mapper.Map<DraftFieldDto>(field);
                    fieldDto.FieldTypeName = field.FieldType.Name;
                    fieldDto.IsRequired = item.IsRequired;
                    fieldDto.FieldCategoryId = item.FieldCategoryId;
                    fieldDto.IsEditable = item.IsEditable;
                    fieldDto.IsChecked = item.IsChecked;
                    // if The field is dropdown => get dropdown Values
                    if (field.DropdownValues is not null)
                        fieldDto.DropdownValuesDto = field.DropdownValues.Select(dropdownValue => dropdownValue.Value).ToList();

                    if (form.FormFields.Any(p => p.FieldId == field.Id))
                        fieldDto.WasSelected = true;

                    result.Add(fieldDto);
                }
                var resultGroupedByCategory = result.GroupBy(item => item.FieldCategoryId);
                var resultGroupedByCategoryDto = new List<DraftFieldsGroupedByCategory>();
                foreach (var item in resultGroupedByCategory)
                {
                    var fieldCategory = await unitOfWork.Repository<FieldCategory>().GetByIdAsync(item.Key);
                    var groupItem = new DraftFieldsGroupedByCategory
                    {
                        FieldCategoryId = fieldCategory.Id,
                        FieldCategoryName = fieldCategory.Name,
                        Fields = item.ToList()
                    };
                    resultGroupedByCategoryDto.Add(groupItem);
                }
                var resultOrderedByCategoryId = resultGroupedByCategoryDto.OrderBy(p => p.FieldCategoryId).ToList();
                return resultOrderedByCategoryId;

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Response<PaginationToReturnDto<FormResponseDto>>> GetAllFormsByTenantIdAsync(int requestTypeId, FormResponseSearchDto dto)
        {
            try
            {

                if (!dto.TenantId.HasValue)
                {
                    Log.Warning("GetAllFormsByTenantIdAsync failed — TenantId is missing. RequestTypeId: {RequestTypeId}", requestTypeId);
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<FormResponseDto>>("Tenant Id is required");
                }

                var trimmedTitle = dto.SearchTerm?.Trim().ToLower();

                Expression<Func<Form, bool>> criteria = f =>
                    f.RequestTypeId == requestTypeId &&
                    f.TenantId == dto.TenantId.Value &&
                    (string.IsNullOrEmpty(trimmedTitle) || (f.Title != null && f.Title.Trim().ToLower().Contains(trimmedTitle)));

                Expression<Func<Form, FormResponseDto>> selector = f => new FormResponseDto
                {
                    FormId = f.Id,
                    Title = f.Title,
                    IsDraft = f.IsDraft,
                    CreationDate = f.CreationDate,
                    CreatedBy = f.CreatedBy
                };

                var formSpec = new GeneralSpecifications<Form>(criteria, dto.PageNumber, dto.PageSize) { AsNoTracking = true };
                formSpec.SetOrderByDescending(f => f.Id);

                var forms = await unitOfWork.Repository<Form>().GetAllWithSpecificationsAndProjection(formSpec, selector);
                var totalCount = await unitOfWork.Repository<Form>().GetCountByCriteriaAsync(criteria);


                var creatorIds = forms
                .Where(f => !string.IsNullOrEmpty(f.CreatedBy))
                .Select(f => f.CreatedBy)
                .Distinct()
                .ToList();

                Dictionary<string, string> userNamesById = new();

                if (creatorIds.Any())
                {
                    var client = new HttpClient();
                    var url = $"{configuration["ApiSettings:Gateway"]}{configuration["MySelfOldUrl:GetUsers"]}";

                    var response = await client.PostAsJsonAsync(url, creatorIds);
                    if (!response.IsSuccessStatusCode)
                    {
                        Log.Warning("GetAllFormsByTenantIdAsync — user lookup failed with status {StatusCode}. RequestTypeId: {RequestTypeId}, TenantId: {TenantId}", response.StatusCode, requestTypeId, dto.TenantId.Value);
                    }
                    else
                    {
                        var usersData = await response.Content.ReadFromJsonAsync<List<UserDataDto>>();
                        userNamesById = usersData?.ToDictionary(u => u.Id, u => u.UserName) ?? new Dictionary<string, string>();
                    }
                }


                
                var formsWithUserNames = forms.Select(f =>
                {
                    f.CreatedBy = userNamesById.TryGetValue(f.CreatedBy ?? string.Empty, out var userName) ? userName : f.CreatedBy;
                    return f;
                }).ToList();
                
                var paginationResult = new PaginationToReturnDto<FormResponseDto>(
                    totalCount,
                    (int)Math.Ceiling((double)totalCount / dto.PageSize),
                    dto.PageSize,
                    dto.PageNumber,
                    formsWithUserNames);

                Log.Information("GetAllFormsByTenantIdAsync successfully retrieved {Count} forms for RequestTypeId {RequestTypeId}, TenantId {TenantId}", formsWithUserNames.Count, requestTypeId, dto.TenantId.Value);

                return new ResponseHandler().Success(paginationResult);

            }
            catch (Exception ex) {
                Log.Error(ex, "GetAllFormsByTenantIdAsync threw an exception. RequestTypeId: {RequestTypeId}, TenantId: {TenantId}", requestTypeId, dto.TenantId);
                return new ResponseHandler().BadRequest<PaginationToReturnDto<FormResponseDto>>("An unexpected error occurred.");

            }
        }
    }
}
