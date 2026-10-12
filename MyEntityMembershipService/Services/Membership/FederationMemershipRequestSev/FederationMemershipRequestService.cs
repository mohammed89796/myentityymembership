using AutoMapper;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.FederationDtos;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos.MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.NewMembershipDtos;
using MyEntityMembershipService.DTOs.Membership.TempRegisterResponseDtos;
using MyEntityMembershipService.Helper;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.FieldRepo;
using MyEntityMembershipService.Repositories.Membership.FormUserResponseRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.FederationMemershipRequestSev
{
    public class FederationMemershipRequestService : IFederationMemershipRequestService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IFieldRepository fieldRepository;
        private readonly IFormUserResponseRepository formUserResponseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFetchService fetchService;
        public FederationMemershipRequestService(IUnitOfWork unitOfWork, IMapper mapper, IFormUserResponseRepository formUserResponseRepository, IHttpContextAccessor httpContextAccessor, IFieldRepository fieldRepository, IFetchService fetchService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.formUserResponseRepository = formUserResponseRepository;
            _httpContextAccessor = httpContextAccessor;
            this.fieldRepository = fieldRepository;
            this.fetchService = fetchService;
        }

        #region add athlete form response   
        public async Task<Response<ReturnAtheleteFormUserResponseDto>> AddAtheleteFormResponse(AddAtheleteFormResponseDto formUserResponseDto)
        {
            try
            {

                Log.Information("Starting AddAtheleteFormResponse with TenentId={TenentId}, BatchId={BatchId}", formUserResponseDto.TenentId, formUserResponseDto.BatchId);
                await unitOfWork.BeginTransactionAsync();
                var newBatch = new Batch();

                if (!formUserResponseDto.BatchId.HasValue || formUserResponseDto.BatchId == 0)
                {
                    Log.Information("Creating new batch for tenant {TenentId}", formUserResponseDto.TenentId);

                    newBatch = new Batch
                    {
                        BatchNumber = $"BATCH-{DateTime.Now:yyyyMMddHHmmss}",
                        TenantId = formUserResponseDto.TenentId,
                        TenantName = formUserResponseDto.TenentName,
                        SuperTenentId = formUserResponseDto.SuperTenentId.Value,
                        SuperEntityName = formUserResponseDto.SuperTenentName,
                        PackageId = formUserResponseDto.PackageId,
                        PackagePrice = formUserResponseDto.PackagePrice,
                        Status = BatchStatus.Draft,
                        RequestTypeId = 6


                    };

                    await unitOfWork.Repository<Batch>().Add(newBatch);
                    await unitOfWork.Save();

                    formUserResponseDto.BatchId = newBatch.Id;
                    Log.Information("Created new batch with ID {BatchId}", newBatch.Id);
                }
                else
                {
                    Log.Information("Using existing batch with ID {BatchId}", formUserResponseDto.BatchId);
                    newBatch.Id = formUserResponseDto.BatchId.Value;
                }
                //karata federation form id changed based on seeding

                var formId = 159;
                Log.Information("Fetching form with ID {FormId}", formId);
                var specs = new GeneralSpecifications<Form>(t => t.Id == formId)
                {
                    Includes =
                   [
                       Iq => Iq.Include(t => t.FormFields).ThenInclude(p => p.Field)

                    ]
                };
                var form = await unitOfWork.Repository<Form>().GetWithSpecifications(specs);
                if (form is null)
                    return new ResponseHandler().BadRequest<ReturnAtheleteFormUserResponseDto>("There is no form with that Id");

                var formFields = form.FormFields.Select(item => item.FieldId).ToList();
                foreach (var fieldResponse in formUserResponseDto.FieldsResponse)
                {
                    if (!formFields.Contains(fieldResponse.FieldId))
                        return new ResponseHandler().BadRequest<ReturnAtheleteFormUserResponseDto>($"There is no field with that Id = {fieldResponse.FieldId}");
                    var field = form.FormFields.FirstOrDefault(field => field.FieldId == fieldResponse.FieldId);
                    if (field.IsRequired.Value && string.IsNullOrEmpty(fieldResponse.Response))
                    {
                        var fieldData = await unitOfWork.Repository<Field>().GetByIdAsync(field.FieldId);
                        return new ResponseHandler().BadRequest<ReturnAtheleteFormUserResponseDto>($"{fieldData.Title} is Required");
                    }
                    if (string.IsNullOrEmpty(fieldResponse.Response))
                    {
                        fieldResponse.Response = string.Empty;
                    }
                    if (fieldResponse.File != null)
                    {
                        var fieldType = await fieldRepository.GetFieldWithDropdownValues(field.FieldId);
                        if (fieldType.FieldTypeId == 6)
                        {
                            fieldResponse.Response = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString")!, "buildupimagedocs", fieldResponse.File);
                        }
                    }


                }
                var formUserResponse = mapper.Map<FormUserResponse>(formUserResponseDto);
                formUserResponse.FormId = 159;
                formUserResponse.PackageId = formUserResponseDto.PackageId;
                formUserResponse.MemberTypeId = 5;
                await unitOfWork.Repository<FormUserResponse>().Add(formUserResponse);
                await unitOfWork.Save();
                Log.Information("Saved FormUserResponse with ID {FormUserResponseId}", formUserResponse.Id);

                var model = new CreateAtheleteMembershipRequestForResponseDto()
                {
                    BacthId = newBatch.Id,
                    TenantId = formUserResponseDto.TenentId,
                    SuperTenentId = formUserResponseDto.SuperTenentId,
                    FormUserResponseId = formUserResponse.Id,
                    FormUserResponseDto = formUserResponseDto,
                };

                var createRequests = await CreateMembershipRequestForResponse(model);
                if (createRequests.Succeeded == false)
                {
                    Log.Warning("Membership creation failed: {Message}", createRequests.Message);
                    return new ResponseHandler().BadRequest<ReturnAtheleteFormUserResponseDto>(createRequests.Message);
                }

                formUserResponseDto = mapper.Map<AddAtheleteFormResponseDto>(formUserResponse);
                foreach (var response in formUserResponseDto.FieldsResponse)
                {
                    var field = await unitOfWork.Repository<Field>().GetByIdAsync(response.FieldId);
                    Log.Information("Setting field name for response with FieldId {FieldId} to {FieldName}", response.FieldId, field.Title);
                    response.FieldName = field?.Title ?? "unknown";
                }
                var result = mapper.Map<ReturnAtheleteFormUserResponseDto>(formUserResponseDto);
                Log.Information("Successfully added athlete form response");
                await unitOfWork.CommitTransactionAsync();

                return new ResponseHandler().Success(result);

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();

                Log.Error(ex, "Error adding athlete form response: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<ReturnAtheleteFormUserResponseDto>(ex.Message);
            }
        }

        private async Task<Response<MembershipRequest>> CreateMembershipRequestForResponse(CreateAtheleteMembershipRequestForResponseDto model)
        {
            try
            {
                Log.Information("Starting CreateMembershipRequestForResponse with FormUserResponseId={FormUserResponseId}, TenantId={TenantId}, SuperTenantId={SuperTenentId}, BatchId={BacthId}",
                model.FormUserResponseId, model.TenantId, model.SuperTenentId, model.BacthId);

                var SuperMembershipRequest = new MembershipRequest();
                var FormUserResponse = await formUserResponseRepository.GetFormUserResponse(model.FormUserResponseId);
                SuperMembershipRequest = mapper.Map<MembershipRequest>(FormUserResponse) ?? new MembershipRequest();
                SuperMembershipRequest.Id = 0;

                SuperMembershipRequest.BatchId = model.BacthId;
                SuperMembershipRequest.TenantId = model.TenantId;
                var validatedTenant = await fetchService.GetTenantEntityAsync(model.TenantId);
                if (validatedTenant is null || validatedTenant.Id == 0)
                {
                    Log.Warning("FormUserResponse with ID {FormUserResponseId} not found", model.FormUserResponseId);

                    return new ResponseHandler().BadRequest<MembershipRequest>("Invalid Tenant Id");
                }

                SuperMembershipRequest.SuperTenantId = model.SuperTenentId;
                SuperMembershipRequest.Status = RequestStatus.Draft;
                SuperMembershipRequest.FormUserResponse = FormUserResponse;
                SuperMembershipRequest.RequestTypeId = 6;
                SuperMembershipRequest.OperationType = OperationType.NewMembership;

                // Validate FileNumber
                var fileNumberAnswer = FormUserResponse.FieldsResponse.FirstOrDefault(a => a.FieldId == 85);

                if (fileNumberAnswer != null && !string.IsNullOrWhiteSpace(fileNumberAnswer.Response))
                {

                    Log.Information("Checking for existing MembershipRequest with FileNumber={FileNumber}", fileNumberAnswer.Response);

                    var existingRequest = await unitOfWork.Repository<MembershipRequest>()
                        .FindByCriteriaAsync(m => m.FileNumber.Trim().ToLower() == fileNumberAnswer.Response.Trim().ToLower() && m.SuperTenantId == model.SuperTenentId);

                    if (existingRequest != null)
                    {
                        Log.Warning("Duplicate FileNumber detected: {FileNumber}", fileNumberAnswer.Response);

                        return new ResponseHandler().BadRequest<MembershipRequest>("File number already exists.");
                    }

                    SuperMembershipRequest.FileNumber = fileNumberAnswer.Response;
                }
                if (SuperMembershipRequest != null && !string.IsNullOrWhiteSpace(SuperMembershipRequest.PhoneNumber))
                {
                    Log.Information("Checking for existing MembershipRequest with PhoneNumber={PhoneNumber}", SuperMembershipRequest.PhoneNumber);


                    var existingRequest = await unitOfWork.Repository<MembershipRequest>()
                        .FindByCriteriaAsync(m => m.PhoneNumber.ToLower() == SuperMembershipRequest.PhoneNumber.ToLower() && m.TenantId == model.TenantId);

                    if (existingRequest != null)
                    {
                        Log.Warning("Duplicate PhoneNumber detected: {PhoneNumber}", SuperMembershipRequest.PhoneNumber);

                        return new ResponseHandler().BadRequest<MembershipRequest>("PhoneNumber already exists.");
                    }

                }




                await unitOfWork.Repository<MembershipRequest>().Add(SuperMembershipRequest);
                Log.Information("Added MembershipRequest for User={Email}", SuperMembershipRequest.Email);

                // Step 3: Create regi user and membership

                var tempRegisterRequestDto = new TempRegisterRequestDto
                {
                    UserName = SuperMembershipRequest.Email,
                    Email = SuperMembershipRequest.Email,
                    TenantId = SuperMembershipRequest.TenantId,
                    PhoneNumber = SuperMembershipRequest.PhoneNumber,
                    NationalityId = string.IsNullOrWhiteSpace(SuperMembershipRequest.NationalId) ? SuperMembershipRequest.PassportNumber : SuperMembershipRequest.NationalId
                };
                Log.Information("Registering temporary user: Email={Email}, Phone={PhoneNumber}",
            tempRegisterRequestDto.Email, tempRegisterRequestDto.PhoneNumber);

                var addUserRequest = await fetchService.TempRegisterUser(tempRegisterRequestDto);

                if (!addUserRequest.Succeeded)
                    return new ResponseHandler().BadRequest<MembershipRequest>(addUserRequest.Message);
                SuperMembershipRequest.UserId = addUserRequest.Data;

                await unitOfWork.Save();

                Log.Information("Saved MembershipRequest successfully with ID={RequestId}", SuperMembershipRequest.Id);

                return new ResponseHandler().Success(SuperMembershipRequest);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating membership request for response: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<MembershipRequest>(ex.Message);
            }
        }
        #endregion

        #region Add Transfer or Loan Form Response

        public async Task<Response<ReturnAtheleteFormUserResponseDto>> AddTransferOrLoanFormResponse(AddAtheleteFormResponseDto formUserResponseDto, int requestTypeId)
        {
            try
            {
                Log.Information("Start AddTransferOrLoanFormResponse | TenantId: {TenantId}, RequestTypeId: {RequestTypeId}", formUserResponseDto.TenentId, requestTypeId);
                await unitOfWork.BeginTransactionAsync();

                var newBatch = new Batch();

                if (!formUserResponseDto.BatchId.HasValue || formUserResponseDto.BatchId == 0)
                {
                    newBatch = new Batch
                    {
                        BatchNumber = $"BATCH-{DateTime.Now:yyyyMMddHHmmss}", // or use a GUID or other numbering logic
                        TenantId = formUserResponseDto.TenentId,
                        TenantName = formUserResponseDto.TenentName,
                        SuperTenentId = formUserResponseDto.SuperTenentId.Value,
                        SuperEntityName = formUserResponseDto.SuperTenentName,
                        PackageId = formUserResponseDto.PackageId,
                        PackagePrice = formUserResponseDto.PackagePrice,
                        Status = BatchStatus.Draft,
                        RequestTypeId = requestTypeId


                    };

                    await unitOfWork.Repository<Batch>().Add(newBatch);
                    await unitOfWork.Save(); // Save to get the auto-generated ID

                    formUserResponseDto.BatchId = newBatch.Id;
                    Log.Information("New Batch Created | BatchId: {BatchId}", newBatch.Id);

                }
                else
                {
                    newBatch.Id = formUserResponseDto.BatchId.Value;
                    Log.Information("Using Existing Batch | BatchId: {BatchId}", newBatch.Id);

                }
                //karata federation form id changed based on seeding
                var formId = requestTypeId == 19 ? 160 : 161; // 16 for Transfer, 162 for Loan
                Log.Information("Fetching Form | FormId: {FormId}", formId);
                var specs = new GeneralSpecifications<Form>(t => t.Id == formId)
                {
                    Includes =
                   [
                       Iq => Iq.Include(t => t.FormFields).ThenInclude(p => p.Field)

                    ]
                };
                var form = await unitOfWork.Repository<Form>().GetWithSpecifications(specs); if (form is null)
                    return new ResponseHandler().BadRequest<ReturnAtheleteFormUserResponseDto>("There is no form with that Id");

                var formFields = form.FormFields.Select(item => item.FieldId).ToList();
                foreach (var fieldResponse in formUserResponseDto.FieldsResponse)
                {
                    if (!formFields.Contains(fieldResponse.FieldId))
                    {
                        Log.Warning("Invalid FieldId in Request | FieldId: {FieldId}", fieldResponse.FieldId);
                        return new ResponseHandler().BadRequest<ReturnAtheleteFormUserResponseDto>($"There is no field with that Id = {fieldResponse.FieldId}");
                    }

                    var field = form.FormFields.First(f => f.FieldId == fieldResponse.FieldId);
                    if (field.IsRequired.Value && string.IsNullOrEmpty(fieldResponse.Response))
                    {
                        var fieldData = await unitOfWork.Repository<Field>().GetByIdAsync(field.FieldId);
                        Log.Warning("Missing Required Field | Field: {FieldTitle}", fieldData.Title);
                        return new ResponseHandler().BadRequest<ReturnAtheleteFormUserResponseDto>($"{fieldData.Title} is Required");
                    }

                    if (fieldResponse.File != null)
                    {
                        var fieldType = await fieldRepository.GetFieldWithDropdownValues(field.FieldId);
                        if (fieldType.FieldTypeId == 6)
                        {
                            Log.Information("Uploading File for FieldId: {FieldId}", fieldResponse.FieldId);
                            fieldResponse.Response = await DocumentSetting.AddToContainerAndGetUrl(
                                Environment.GetEnvironmentVariable("StorageConnectionString")!,
                                "buildupimagedocs",
                                fieldResponse.File);
                        }
                    }

                    fieldResponse.Response ??= string.Empty;
                }
                var formUserResponse = mapper.Map<FormUserResponse>(formUserResponseDto);
                formUserResponse.FormId = formId;
                formUserResponse.PackageId = formUserResponseDto.PackageId;
                formUserResponse.MemberTypeId = 5;
                await unitOfWork.Repository<FormUserResponse>().Add(formUserResponse);
                await unitOfWork.Save();
                Log.Information("Saved FormUserResponse | Id: {FormUserResponseId}", formUserResponse.Id);

                var model = new CreateAtheleteMembershipRequestForResponseDto()
                {
                    BacthId = newBatch.Id,
                    TenantId = formUserResponseDto.TenentId,
                    SuperTenentId = formUserResponseDto.SuperTenentId,
                    FormUserResponseId = formUserResponse.Id,
                    FormUserResponseDto = formUserResponseDto,
                };

                var createRequests = await CreateTransOrLoanferRequestForResponse(model, requestTypeId);
                if (createRequests.Succeeded == false)
                {
                    return new ResponseHandler().BadRequest<ReturnAtheleteFormUserResponseDto>(createRequests.Message);
                }

                formUserResponseDto = mapper.Map<AddAtheleteFormResponseDto>(formUserResponse);
                foreach (var response in formUserResponseDto.FieldsResponse)
                {
                    var field = await unitOfWork.Repository<Field>().GetByIdAsync(response.FieldId);
                    response.FieldName = field?.Title ?? "unknown";
                }
                var result = mapper.Map<ReturnAtheleteFormUserResponseDto>(formUserResponseDto);
                await unitOfWork.CommitTransactionAsync();

                return new ResponseHandler().Success(result);

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();

                Log.Error(ex, "Error in AddTransferOrLoanFormResponse: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<ReturnAtheleteFormUserResponseDto>(ex.Message);
            }
        }
        private async Task<Response<MembershipRequest>> CreateTransOrLoanferRequestForResponse(CreateAtheleteMembershipRequestForResponseDto model, int requstTypeId)
        {
            try
            {

                var SuperMembershipRequest = new MembershipRequest();
                var FormUserResponse = await formUserResponseRepository.GetFormUserResponse(model.FormUserResponseId);


                var SuperMembershipRequestfromres = mapper.Map<MembershipRequest>(FormUserResponse);

                SuperMembershipRequest = mapper.Map<MembershipRequest>(FormUserResponse);
                if (!string.IsNullOrWhiteSpace(SuperMembershipRequestfromres.FileNumber))
                {
                    var membershipsWithFile = await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Membership>()
                        .FindByCriteriaAsync(m => m.FileNumber != null && m.FileNumber.ToLower() == SuperMembershipRequestfromres.FileNumber.ToLower() && m.TenantId == SuperMembershipRequestfromres.FromTenantId && m.MembershipStatus == MembershipStatus.Active);

                    if (membershipsWithFile != null)
                    {
                        //  Check if a request  already exists for same Membership, tenant, and request type

                        var existingRequest = await unitOfWork.Repository<MembershipRequest>().FindByCriteriaAsync(r =>
                            r.MembershipId == membershipsWithFile.Id &&
                            r.TenantId == model.TenantId &&
                            r.RequestTypeId == requstTypeId &&
                            !r.IsDeleted);

                        if (existingRequest is not null)
                        {
                            return new ResponseHandler().BadRequest<MembershipRequest>("there is an already active requst for this member");
                        }
                        SuperMembershipRequest = mapper.Map<MembershipRequest>(membershipsWithFile);
                        SuperMembershipRequest.MembershipId = membershipsWithFile.Id;

                    }
                }

                SuperMembershipRequest.Id = 0;
                SuperMembershipRequest.BatchId = model.BacthId;
                SuperMembershipRequest.TenantId = model.TenantId;
                SuperMembershipRequest.FromTenantId = SuperMembershipRequestfromres.FromTenantId;


                var validatedTenant = await fetchService.GetTenantEntityAsync(model.TenantId);
                if (validatedTenant is null || validatedTenant.Id == 0)
                {
                    return new ResponseHandler().BadRequest<MembershipRequest>("Invalid Tenant Id");
                }
                SuperMembershipRequest.SuperTenantId = model.SuperTenentId;
                SuperMembershipRequest.Status = RequestStatus.Draft;
                SuperMembershipRequest.FormUserResponse = FormUserResponse;
                SuperMembershipRequest.RequestTypeId = requstTypeId;
                SuperMembershipRequest.OperationType = requstTypeId == 19 ? OperationType.Transfer : OperationType.Loan;



                await unitOfWork.Repository<MembershipRequest>().Add(SuperMembershipRequest);
                await unitOfWork.Save();

                return new ResponseHandler().Success(SuperMembershipRequest);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequest>(ex.Message);
            }
        }

        #endregion

        public async Task<Response<MembershipDto>> ValidateAthleteMembership(int areaId, int entityId, string fileNumber, string nationalId)
        {
            // Get the current entity
            var entity = await unitOfWork.Repository<Tenant>()
                        .FindByCriteriaAsync(t => t.Id == entityId);

            if (entity == null)
                return new ResponseHandler().NotFound<MembershipDto>("Entity not found.");

            var federationTenantId = entity.SuperEntityId ?? entity.Id;

            var federationTenants = await unitOfWork.Repository<Tenant>()
                .FindAllByCriteriaAsync(t => t.SuperEntityId == federationTenantId || t.Id == federationTenantId);
            var federationTenantIds = federationTenants.Select(t => t.Id).ToList();

            var areaTenants = await unitOfWork.Repository<TenantArea>()
                .FindAllByCriteriaAsync(ta => ta.AreaId == areaId);
            var areaTenantIds = areaTenants.Select(ta => ta.TenantId).ToList();

            var allMembershipsWithFile = await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Membership>()
                        .FindAllByCriteriaAsync(m => m.FileNumber != null
                        && m.FileNumber.ToLower() == fileNumber.ToLower()
                        && (
                               (m.NationalId != null && m.NationalId.ToLower().Trim() == nationalId.ToLower().Trim()) ||
                               (m.PassportNumber != null && m.PassportNumber.ToLower().Trim() == nationalId.ToLower().Trim())
                           )
                        && m.MembershipStatus == MembershipStatus.Active);

            if (!allMembershipsWithFile.Any())
            {
                return new ResponseHandler().NotFound<MembershipDto>(
                    "This athlete is not registered in the system.");
            }

            var federationMemberships = allMembershipsWithFile
                .Where(m => federationTenantIds.Contains(m.TenantId))
                .ToList();

            if (!federationMemberships.Any())
            {
                return new ResponseHandler().NotFound<MembershipDto>(
                    "This athlete is not registered in the federation.");
            }

            var entityMembership = federationMemberships
                .FirstOrDefault(m => m.TenantId == entityId);

            if (entityMembership == null)
            {
                return new ResponseHandler().BadRequest<MembershipDto>(
                    "This athlete is not registered under this entity.");
            }

            // ✅ All checks passed
            var dto = mapper.Map<MembershipDto>(entityMembership);

            dto.NationalIDPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.NationalIDPhoto);
            dto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(dto.UserPhoto);

            return new ResponseHandler().Success(dto);
        }
        public async Task<Response<MembershipRequestDto>> UpdateFieldResponsesForRequestAsync(int membershipRequestId, List<AtheleteFieldResponseDto> updatedResponses, PackageMembershipRequestAccessDto? requestAccessDto)
        {
            try

            {
                var spec = new GeneralSpecifications<MembershipRequest>(r => r.Id == membershipRequestId)
                {
                    Includes = [
                        iq => iq.Include(r => r.FormUserResponse)
                        .ThenInclude(rf => rf.FieldsResponse)
                        .ThenInclude(ff => ff.Field)
                        .Include(r => r.MembershipRequestAccesses)
                    ]
                };

                var request = await unitOfWork.Repository<MembershipRequest>().GetWithSpecifications(spec);

                if (request == null || request.FormUserResponse == null)
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Membership request or FormUserResponse not found.");

                var formResponse = request.FormUserResponse;
                formResponse.FieldsResponse ??= new List<FieldResponse>();


                var specs = new GeneralSpecifications<Form>(t => t.Id == formResponse.FormId)
                {
                    Includes =
                   [
                       Iq => Iq.Include(t => t.FormFields).ThenInclude(p => p.Field)

                    ]
                };
                var form = await unitOfWork.Repository<Form>().GetWithSpecifications(specs); if (form == null)
                    return new ResponseHandler().BadRequest<MembershipRequestDto>("Associated form not found.");

                var validFieldIds = form.FormFields.Select(f => f.FieldId).ToHashSet();
                var requiredFieldIds = form.FormFields.Where(f => f.IsRequired == true).Select(f => f.FieldId).ToHashSet();

                foreach (var dto in updatedResponses)
                {
                    var validation = await ValidateFieldInputAsync(dto, validFieldIds, requiredFieldIds);
                    if (validation != null) return validation;

                    string responseValue = await HandleFileUploadIfNeededAsync(dto);
                    dto.Response = responseValue;
                    if (dto.FieldId == 85 && !string.IsNullOrWhiteSpace(dto.Response))
                    {
                        if (requestAccessDto == null)
                        {
                            bool isDuplicate = await IsFileNumberDuplicateAsync(dto.Response, request.TenantId, membershipRequestId);
                            if (isDuplicate)
                                return new ResponseHandler().BadRequest<MembershipRequestDto>("File number already exists.");

                        }

                        request.FileNumber = dto.Response;
                    }

                    await UpdateOrAddFieldResponseAsync(formResponse, dto, responseValue);
                }

                ApplyUpdatedFieldsToRequest(updatedResponses, formResponse, request);

                unitOfWork.Repository<MembershipRequest>().Update(request);
                // Get current venue accesses for this request

                //  If access data provided, attach to request
                if (requestAccessDto?.VenueIds != null && requestAccessDto.VenueIds.Any())
                {
                    request.MembershipRequestAccesses ??= new List<MembershipRequestAccess>();

                    var existingAccesses = request.MembershipRequestAccesses
                   .Where(a => a.VenueId.HasValue)
                   .ToList();

                    var newVenueIds = requestAccessDto.VenueIds.Distinct().ToList();

                    // 1. Delete old accesses not in the new list
                    var toRemove = existingAccesses
                        .Where(a => !newVenueIds.Contains(a.VenueId.Value))
                        .ToList();

                    foreach (var remove in toRemove)
                    {
                        request.MembershipRequestAccesses.Remove(remove);
                    }

                    // 2. Add new accesses that are not already there
                    foreach (var venueId in newVenueIds)
                    {
                        var alreadyExists = existingAccesses.Any(a => a.VenueId == venueId);
                        if (!alreadyExists)
                        {
                            var accessExists = await unitOfWork.Repository<AccessedFacilitiesVenues>()
                                .FindByCriteriaAsync(a =>
                                    a.PackageId == requestAccessDto.PackageId &&
                                    a.VenueId == venueId);

                            if (accessExists == null)
                            {
                                return new ResponseHandler().BadRequest<MembershipRequestDto>(
                                    $"Venue {venueId} is not accessible in package {requestAccessDto.PackageId}.");
                            }

                            request.MembershipRequestAccesses.Add(new MembershipRequestAccess
                            {
                                MembershipRequestId = request.Id,
                                BranchId = accessExists.BranchId,
                                FacilityId = accessExists.FacilityId,
                                VenueId = accessExists.VenueId
                            });
                        }
                    }

                    // 3. Update package
                    request.PackageId = requestAccessDto.PackageId;
                    unitOfWork.Repository<MembershipRequest>().Update(request);
                }
                else if (requestAccessDto?.VenueIds == null || !requestAccessDto.VenueIds.Any())
                {
                    request.MembershipRequestAccesses = new List<MembershipRequestAccess>();
                    unitOfWork.Repository<MembershipRequest>().Update(request);

                }
                if (updatedResponses.Any(x => new List<int> { 56, 57, 63 }.Contains(x.FieldId)))
                {
                    //  Update regi user and membership
                    Log.Information($"the request try to Update User{request.UserId} Data(Phobe number nationId or email with requestId :)", request.Id);

                    var UpdatedTempRegisterRequestDto = new UpdateTempUserRequestDto
                    {
                        UserId = request.UserId ?? string.Empty,
                        UserName = request.Email,
                        Email = request.Email,
                        TenantId = request.TenantId,
                        PhoneNumber = request.PhoneNumber,
                        NationalityId = request.NationalId
                    };

                    var UpdatedUserRequest = await fetchService.UpdateempRegisterUser(UpdatedTempRegisterRequestDto);
                    Log.Information($"the User{request.UserId} Data(Phobe number nationId or email with requestId :)Updated sucssfully", request.Id);

                    if (!UpdatedUserRequest.Succeeded)
                    {
                        Log.Information($"error in updating User{request.UserId} Data(Phobe number nationId or email with requestId :)Updated sucssfully", UpdatedUserRequest.Message);
                        return new ResponseHandler().BadRequest<MembershipRequestDto>(UpdatedUserRequest.Message);
                    }
                }


                await unitOfWork.Save();

                var result = mapper.Map<MembershipRequestDto>(request);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipRequestDto>(
                    $"Error updating field responses: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        #region Add Athletes By BulkUpload 
        public async Task<Response<List<ReturnAtheleteFormUserResponseDto>>> AddAthletesFromExcelAsync(ExcelAtheleteRowDto sharedDto)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                var formId = 159;

                // Step 1: Parse Excel
                var parsedAthletes = await ParseExcelFileToAthleteDtos(sharedDto.file, formId);

                // Step 2: Create or use existing batch
                var batch = new Batch
                {
                    BatchNumber = $"BATCH-{DateTime.Now:yyyyMMddHHmmss}",
                    TenantId = sharedDto.TenentId,
                    TenantName = sharedDto.TenentName,
                    SuperTenentId = sharedDto.SuperTenentId!.Value,
                    SuperEntityName = sharedDto.SuperTenentName,
                    PackageId = sharedDto.PackageId,
                    PackagePrice = sharedDto.PackagePrice,
                    Status = BatchStatus.Draft,
                    RequestTypeId = 6
                };

                await unitOfWork.Repository<Batch>().Add(batch);
                await unitOfWork.Save();

                sharedDto.BatchId = batch.Id;

                // Step 3: Load form for validation

                var specs = new GeneralSpecifications<Form>(t => t.Id == formId)
                {
                    Includes =
                   [
                       Iq => Iq.Include(t => t.FormFields).ThenInclude(p => p.Field)

                    ]
                };
                var form = await unitOfWork.Repository<Form>().GetWithSpecifications(specs); if (form is null)
                    return new ResponseHandler().BadRequest<List<ReturnAtheleteFormUserResponseDto>>("Form not found.");

                var formFields = form.FormFields.Select(ff => ff.FieldId).ToHashSet();
                var resultList = new List<ReturnAtheleteFormUserResponseDto>();
                var membershipModels = new List<CreateAtheleteMembershipRequestForResponseDto>();

                // Step 4: Save each athlete
                foreach (var athleteDto in parsedAthletes)
                {
                    var dto = new AddAtheleteFormResponseDto
                    {
                        BatchId = sharedDto.BatchId,
                        TenentId = sharedDto.TenentId,
                        TenentName = sharedDto.TenentName,
                        SuperTenentId = sharedDto.SuperTenentId,
                        SuperTenentName = sharedDto.SuperTenentName,
                        PackageId = sharedDto.PackageId,
                        PackagePrice = sharedDto.PackagePrice,
                        FieldsResponse = athleteDto.FieldsResponse
                    };

                    // Validate fields
                    foreach (var fieldResponse in dto.FieldsResponse)
                    {
                        if (!formFields.Contains(fieldResponse.FieldId))
                            return new ResponseHandler().BadRequest<List<ReturnAtheleteFormUserResponseDto>>($"Invalid field ID: {fieldResponse.FieldId}");

                        var field = form.FormFields.First(f => f.FieldId == fieldResponse.FieldId);

                        if (field.IsRequired == true && string.IsNullOrWhiteSpace(fieldResponse.Response))
                        {
                            var fieldData = await unitOfWork.Repository<Field>().GetByIdAsync(field.FieldId);
                            return new ResponseHandler().BadRequest<List<ReturnAtheleteFormUserResponseDto>>($"{fieldData.Title} is required.");
                        }

                        if (string.IsNullOrEmpty(fieldResponse.Response))
                            fieldResponse.Response = string.Empty;
                    }

                    var formUserResponse = mapper.Map<FormUserResponse>(dto);
                    formUserResponse.FormId = formId;
                    formUserResponse.PackageId = dto.PackageId;
                    formUserResponse.MemberTypeId = 5;

                    await unitOfWork.Repository<FormUserResponse>().Add(formUserResponse);
                    await unitOfWork.Save();

                    membershipModels.Add(new CreateAtheleteMembershipRequestForResponseDto
                    {
                        BacthId = batch.Id,
                        TenantId = dto.TenentId,
                        SuperTenentId = dto.SuperTenentId,
                        FormUserResponseId = formUserResponse.Id,
                        FormUserResponseDto = dto
                    });

                    var finalDto = mapper.Map<AddAtheleteFormResponseDto>(formUserResponse);
                    foreach (var response in finalDto.FieldsResponse)
                    {
                        var field = await unitOfWork.Repository<Field>().GetByIdAsync(response.FieldId);
                        response.FieldName = field.Title;
                    }

                    var result = mapper.Map<ReturnAtheleteFormUserResponseDto>(finalDto);
                    resultList.Add(result);
                }

                // Step 5: Call bulk membership creation
                var createReq = await CreateMembershipRequestsForResponses(membershipModels);
                if (!createReq.Succeeded)
                    return new ResponseHandler().BadRequest<List<ReturnAtheleteFormUserResponseDto>>(createReq.Message);



                await unitOfWork.CommitTransactionAsync();
                return new ResponseHandler().Success(resultList, "Athletes added successfully.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();
                Log.Error(ex, "Error in AddAthletesFromExcelAsync: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<List<ReturnAtheleteFormUserResponseDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<ExcelAtheleteRowDto>> ParseExcelFileToAthleteDtos(IFormFile file, int formId)
        {
            var list = new List<ExcelAtheleteRowDto>();
            var specs = new GeneralSpecifications<Form>(t => t.Id == formId)
            {
                Includes =
                   [
                       Iq => Iq.Include(t => t.FormFields).ThenInclude(p => p.Field)

                    ]
            };
            var form = await unitOfWork.Repository<Form>().GetWithSpecifications(specs);
            if (form?.FormFields == null || !form.FormFields.Any())
                throw new Exception("Form fields not found.");

            var headerToFieldId = form.FormFields.ToDictionary(f => f.Field.Title.Trim(), f => f.Field.Id);

            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheet("Template");

            var headers = sheet.Row(1).Cells().Select(c => c.Value.ToString()?.Trim()).ToList();

            foreach (var row in sheet.RowsUsed().Skip(1)) // Skip header
            {
                var dto = new ExcelAtheleteRowDto
                {
                    FieldsResponse = new List<AtheleteFieldResponseDto>()
                };

                for (int i = 0; i < headers.Count; i++)
                {
                    var header = headers[i];
                    var cell = row.Cell(i + 1);
                    var value = cell.GetFormattedString()?.Trim();

                    if (!string.IsNullOrEmpty(header) && headerToFieldId.ContainsKey(header))
                    {
                        dto.FieldsResponse.Add(new AtheleteFieldResponseDto
                        {
                            FieldName = header,
                            FieldId = headerToFieldId[header],
                            Response = value
                        });
                    }
                }

                list.Add(dto);
            }

            return list;
        }

        private async Task<Response<List<MembershipRequest>>> CreateMembershipRequestsForResponses(List<CreateAtheleteMembershipRequestForResponseDto> models)
        {
            var results = new List<MembershipRequest>();
            var tempUsersToRegister = new List<TempRegisterRequestDto>();
            var requests = new List<(MembershipRequest Request, string Email)>();

            // Prepare mapping and registration list
            foreach (var model in models)
            {
                var formUserResponse = await formUserResponseRepository.GetFormUserResponse(model.FormUserResponseId);
                if (formUserResponse == null)
                    return new ResponseHandler().BadRequest<List<MembershipRequest>>($"FormUserResponse not found for ID {model.FormUserResponseId}");

                // Map to MembershipRequest
                var request = mapper.Map<MembershipRequest>(formUserResponse);
                request.Id = 0;
                request.BatchId = model.BacthId;
                request.TenantId = model.TenantId;
                request.SuperTenantId = model.SuperTenentId;
                request.Status = RequestStatus.Draft;
                request.FormUserResponse = formUserResponse;
                request.RequestTypeId = 6;
                request.OperationType = OperationType.NewMembership;

                var email = request.Email?.Trim()?.ToLower();
                var phone = request.PhoneNumber;
                var nationalId = request.NationalId;

                if (string.IsNullOrWhiteSpace(email))
                    return new ResponseHandler().BadRequest<List<MembershipRequest>>($"Missing email in FormUserResponse ID {model.FormUserResponseId}");

                // Add only if not already prepared
                if (!tempUsersToRegister.Any(u => u.Email?.ToLower() == email))
                {
                    tempUsersToRegister.Add(new TempRegisterRequestDto
                    {
                        UserName = email,
                        Email = email,
                        TenantId = request.TenantId,
                        PhoneNumber = phone,
                        NationalityId = nationalId
                    });
                }

                // Check file number duplication
                var fileNumberAnswer = formUserResponse.FieldsResponse.FirstOrDefault(a => a.FieldId == 85);
                if (fileNumberAnswer != null && !string.IsNullOrWhiteSpace(fileNumberAnswer.Response))
                {
                    var specs = new GeneralSpecifications<Data.Membership.Entities.Membership>(m =>
                        m.FileNumber.Trim().ToLower() == fileNumberAnswer.Response.Trim().ToLower() &&
                        m.Tenant.SuperEntityId == model.SuperTenentId && m.MembershipStatus == MembershipStatus.Active &&
                        !m.IsDeleted
                    )
                    {
                        Includes =
                        [
                            m => m.Include(x => x.Tenant)
                        ]
                    };

                    var existingMembership = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                        .GetWithSpecifications(specs);


                    if (existingMembership != null)
                        return new ResponseHandler().BadRequest<List<MembershipRequest>>($"File number '{fileNumberAnswer.Response}' already exists In The Federation Sytem.");

                    request.FileNumber = fileNumberAnswer.Response;
                }

                requests.Add((request, email));
            }

            // Bulk register users
            var registrationResult = await fetchService.BulkRegisterUsersAsync(tempUsersToRegister);
            if (!registrationResult.Succeeded)
                return new ResponseHandler().BadRequest<List<MembershipRequest>>(registrationResult.Message);

            // Build map from email to UserId
            var emailToUserIdMap = registrationResult.Data
                .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                .ToDictionary(x => x.Email!.Trim().ToLower(), x => x.UserId);

            // Assign UserId and persist requests
            foreach (var (request, email) in requests)
            {
                if (!emailToUserIdMap.TryGetValue(email, out var userId))
                    return new ResponseHandler().BadRequest<List<MembershipRequest>>($"UserId not returned for email '{email}'");

                request.UserId = userId;

                await unitOfWork.Repository<MembershipRequest>().Add(request);
                results.Add(request);
            }

            await unitOfWork.Save();
            return new ResponseHandler().Success(results, "All membership requests created successfully.");
        }
        #endregion
        #region Validate Field Input, Handle File Upload, Check File Number Duplicate, Update Or Add Field Response
        private async Task<Response<MembershipRequestDto>?> ValidateFieldInputAsync(AtheleteFieldResponseDto dto, HashSet<int> validFieldIds, HashSet<int> requiredFieldIds)
        {
            if (!validFieldIds.Contains(dto.FieldId))
                return new ResponseHandler().BadRequest<MembershipRequestDto>($"Invalid field ID: {dto.FieldId}");

            if (requiredFieldIds.Contains(dto.FieldId) &&
                string.IsNullOrWhiteSpace(dto.Response) && dto.File == null)
            {
                var field = await unitOfWork.Repository<Field>().GetByIdAsync(dto.FieldId);
                return new ResponseHandler().BadRequest<MembershipRequestDto>($"{field?.Title ?? "Field"} is required.");
            }

            return null;
        }
        private async Task<string> HandleFileUploadIfNeededAsync(AtheleteFieldResponseDto dto)
        {
            if (dto.File == null) return dto.Response?.Trim() ?? string.Empty;

            var fieldWithType = await fieldRepository.GetFieldWithDropdownValues(dto.FieldId);
            if (fieldWithType?.FieldTypeId == 6)
            {
                return await DocumentSetting.AddToContainerAndGetUrl(
                    Environment.GetEnvironmentVariable("StorageConnectionString")!,
                    "buildupimagedocs",
                    dto.File
                );
            }

            return dto.Response?.Trim() ?? string.Empty;
        }
        private async Task<bool> IsFileNumberDuplicateAsync(string fileNumber, int tenantId, int currentRequestId)
        {
            var existingRequest = await unitOfWork.Repository<MembershipRequest>()
                .FindByCriteriaAsync(m =>
                    m.FileNumber.ToLower() == fileNumber.ToLower() &&
                    m.TenantId == tenantId &&
                    m.Id != currentRequestId);

            return existingRequest != null;
        }
        private async Task UpdateOrAddFieldResponseAsync(FormUserResponse formResponse, AtheleteFieldResponseDto dto, string responseValue)
        {
            var existing = formResponse.FieldsResponse.FirstOrDefault(fr => fr.FieldId == dto.FieldId);

            if (existing != null)
            {
                existing.Response = responseValue;
                existing.UnitId = dto.UnitId;
                unitOfWork.Repository<FieldResponse>().Update(existing);
            }
            else
            {
                var newResponse = new FieldResponse
                {
                    FormUserResponseId = formResponse.Id,
                    FieldId = dto.FieldId,
                    Response = responseValue,
                    UnitId = dto.UnitId
                };

                await unitOfWork.Repository<FieldResponse>().Add(newResponse);
                formResponse.FieldsResponse.Add(newResponse);
            }
        }
        private void ApplyUpdatedFieldsToRequest(List<AtheleteFieldResponseDto> updatedResponses, FormUserResponse formUserResponse, MembershipRequest request)
        {
            foreach (var dto in updatedResponses)
            {
                var field = formUserResponse.FieldsResponse
                    .FirstOrDefault(fr => fr.FieldId == dto.FieldId)?.Field;

                if (field == null || string.IsNullOrWhiteSpace(field.MapField))
                    continue;

                var property = typeof(MembershipRequest).GetProperty(field.MapField);
                if (property != null && property.CanWrite)
                {
                    string? valueToMap = dto.Response;
                    object? convertedValue = ConvertHelper.ConvertToPropertyType(property.PropertyType, valueToMap);
                    property.SetValue(request, convertedValue);
                }
            }
        }
        private async Task<Response<string>?> TryRegisterUserAsync(MembershipRequest request)
        {
            var tempRegisterRequestDto = new TempRegisterRequestDto
            {
                UserName = request.Email,
                Email = request.Email,
                TenantId = request.TenantId,
                PhoneNumber = request.PhoneNumber,
                NationalityId = request.NationalId
            };

            var addUserResponse = await fetchService.TempRegisterUser(tempRegisterRequestDto);

            if (!addUserResponse.Succeeded)
            {
                return new ResponseHandler().BadRequest<string>(addUserResponse.Message);
            }

            request.UserId = addUserResponse.Data;
            return null;
        }


        #endregion

        #region Swimming

        public async Task<Response<MembershipRequestDto>> AddFormSubmissionAsync(FormSubmissionDto dto)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();

                // process parent (and children recursively)
                var membershipRequest = await ProcessFormSubmissionAsync(dto);

                await unitOfWork.CommitTransactionAsync();

                var result = mapper.Map<MembershipRequestDto>(membershipRequest);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();
                Log.Error(ex, "Error adding form submission: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<MembershipRequestDto>(ex.Message);
            }
        }

        private async Task<MembershipRequest> ProcessFormSubmissionAsync(FormSubmissionDto dto, int? parentRequestId = null)
        {

            var Tenant = await unitOfWork.Repository<Tenant>().GetByIdAsync(dto.TenantId);
            var newBatch = new Batch();
            if (!dto.BatchId.HasValue || dto.BatchId == 0)
            {
                newBatch = new Batch
                {
                    BatchNumber = $"BATCH-{DateTime.Now:yyyyMMddHHmmss}",
                    TenantId = dto.TenantId,
                    TenantName =Tenant.Name,
                    PackageId = dto.PackageId,
                    Status = BatchStatus.Draft,
                    RequestTypeId = 6
                };

                await unitOfWork.Repository<Batch>().Add(newBatch);
                await unitOfWork.Save();
                dto.BatchId = newBatch.Id;
            }
            else
            {
                newBatch.Id = dto.BatchId.Value;
            }

            // 1. Load form metadata
            var specs = new GeneralSpecifications<Form>(t => t.Id == dto.FormId)
            {
                Includes = [f => f.Include(t => t.FormFields).ThenInclude(ff => ff.Field)]
            };
            var form = await unitOfWork.Repository<Form>().GetWithSpecifications(specs)
                ?? throw new InvalidOperationException("Invalid FormId");

            var validFieldIds = form.FormFields.Select(ff => ff.FieldId).ToHashSet();
            var requiredFieldIds = form.FormFields
                .Where(ff => ff.IsRequired.GetValueOrDefault())
                .Select(ff => ff.FieldId)
                .ToHashSet();

            // 2. Validate/process responses
            foreach (var response in dto.FieldsResponses)
            {
                var validationResult = await ValidateFieldInputAsync(response, validFieldIds, requiredFieldIds);
                if (validationResult != null)
                    throw new InvalidOperationException(validationResult.Message);

                response.Response = await HandleFileUploadIfNeededAsync(response);
            }

            // 3. Save FormUserResponse
            var formUserResponse = new FormUserResponse
            {
                FormId = dto.FormId,
                PackageId = dto.PackageId,
                FieldsResponse = dto.FieldsResponses.Select(r => new FieldResponse
                {
                    FieldId = r.FieldId,
                    Response = r.Response ?? string.Empty,
                    UnitId = r.UnitId
                }).ToList(),
                UserId = dto.UserId,
            };
            await unitOfWork.Repository<FormUserResponse>().Add(formUserResponse);
            await unitOfWork.Save();

            // 4. Create MembershipRequest
            var membershipRequest = mapper.Map<MembershipRequest>(formUserResponse) ?? new MembershipRequest();
            membershipRequest.Id = 0;
            membershipRequest.TenantId = dto.TenantId;
            membershipRequest.BatchId = dto.BatchId;
            membershipRequest.Status = RequestStatus.Draft;
            membershipRequest.OperationType = OperationType.NewMembership;
            membershipRequest.FormUserResponse = formUserResponse;
            membershipRequest.SuperMembershipRequestId = parentRequestId;
            membershipRequest.UserId = dto.UserId;
            membershipRequest.SubmationPlatform = SubmationPlatform.MobileUser;

             if (parentRequestId == null && form.RequestTypeId == 6)
            {
                membershipRequest.MemberTypeId = 5;
                membershipRequest.RequestTypeId = 6;
                membershipRequest.PackageId = dto.PackageId;
                membershipRequest.Status=RequestStatus.Pending;
            }
            else if(parentRequestId == null)
            {
                membershipRequest.MemberTypeId = 5;
                membershipRequest.RequestTypeId = 21;
                membershipRequest.PackageId = dto.PackageId;
                membershipRequest.IsParent = true;

            }
            else
            {
                membershipRequest.MemberTypeId = 17;
                membershipRequest.RequestTypeId = 21;
                membershipRequest.PackageId = null;

            }

            await unitOfWork.Repository<MembershipRequest>().Add(membershipRequest);
            await unitOfWork.Save();

            // 5. Recursively handle children
            foreach (var childDto in dto.Children)
            {
                childDto.BatchId = dto.BatchId;
                await ProcessFormSubmissionAsync(childDto, membershipRequest.Id);
            }

            return membershipRequest;
        }

        #endregion 

    }
}
