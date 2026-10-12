using AutoMapper;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Entity.FederationDtos;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;
using MyEntityMembershipService.DTOs.Membership.AdditionalConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.AreaDtos;
using MyEntityMembershipService.DTOs.Membership.CancellationConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.ConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.DTOs.Membership.FederationDtos;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.FieldsDto;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.DTOs.Membership.FreezeConfigurtionDtos;
using MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.GetPackageMemberTypesDtos;
using MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.NewMembershipConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.NewMembershipDtos;
using MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos;
using MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos.FedrationEntitiesPaymentRequests;
using MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos;
using MyEntityMembershipService.DTOs.Membership.PreRegisteredUsersDtos;
using MyEntityMembershipService.DTOs.Membership.RemovalConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.RenewalConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.RequestTypeCategoryDtos;
using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;
using MyEntityMembershipService.DTOs.Membership.Swimming;
using MyEntityMembershipService.DTOs.Membership.TournamentResultDtos;
using MyEntityMembershipService.DTOs.Membership.TransactionDtos;
using MyEntityMembershipService.DTOs.Membership.UnitDtos;
using MyEntityMembershipService.Extensions;
using PaymentRequestDto = MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos.PaymentRequestDto;
using SubMemberCategoryDto = MyEntityMembershipService.DTOs.Membership.PackageDtos.SubMemberCategoryDto;

namespace MyEntityMembershipService.Helper.MappingProfile.Membership
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<AddMembershipRequestDto, MembershipRequest>().ReverseMap();
            CreateMap<CreateMembershipRequestDto, MembershipRequest>().ReverseMap();
            //CreateMap<AddTenantDto, Tenant>().ReverseMap();
            //CreateMap<TenantDto, Tenant>().ReverseMap();

            CreateMap<MembershipRequest, ReturnMembershipRequestDto>().ReverseMap();
            CreateMap<MembershipRequestDto, MembershipRequest>()
               .ForMember(dest => dest.SubMembers, opt => opt.MapFrom(src => src.SubMembers))
             .ForMember(dest => dest.RequestStatusHistories, opt => opt.MapFrom(src => src.LastStatusUpdate))

                .ReverseMap();
            CreateMap<CraetMembershipDto, Data.Membership.Entities.Membership>().ReverseMap();

            CreateMap<MembershipDto, Data.Membership.Entities.Membership>().ReverseMap();
            CreateMap<MembershipRequest, MembershipRequestDto>()
              .ForMember(dest => dest.TenantName,
                  opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.Name : null)) 
              .ForMember(dest => dest.PaymentRequestDto,
                  opt => opt.MapFrom(src => src.PaymentRequests));

            CreateMap<PaymentRequestDto, PaymentRequest>().ReverseMap();
            //.ForMember(dest => dest.TenantDto, opt => opt.MapFrom(src => src.Tenant));

            CreateMap<Transaction, TransactionDto>().ReverseMap();



            CreateMap<RequestType, RequestTypeDto>().ReverseMap();
            CreateMap<RequestTypeCategory, RequestTypeCategoryDto>();

            CreateMap<AddMembershipWaiverRequestDto, MembershipRequest>().ReverseMap();
            CreateMap<AddMembershipSeparationRequestDto, MembershipRequest>().ReverseMap();
            CreateMap<AddMembershipModificationRequestDto, MembershipRequest>().ReverseMap();
            CreateMap<Data.Membership.Entities.Membership, CurrentMembershipDto>().ReverseMap();
            CreateMap<MembershipRequest, CurrentMembershipRequestDto>().ReverseMap();


            CreateMap<MembershipRequest, GetMembershipRequestDto>()
                .ForMember(dest => dest.Status, o => o.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.MemberTypeName, o => o.MapFrom(src => src.MemberType.Name));

            CreateMap<MembershipRequestComment, ActivityDto>()
                .ForMember(activity => activity.Comments, o => o.MapFrom(requestComment => requestComment.CommentsAndJustfications));
            CreateMap<PaymentRequest, ActivityDto>().ReverseMap();
            CreateMap<SubMemberDto, MembershipRequest>().ReverseMap();
            CreateMap<SubMemberDto, Data.Membership.Entities.Membership>()
                .ForMember(dist => dist.MembershipStatus, o => o.MapFrom(src => src.Status))
                .ReverseMap();
            CreateMap<FieldTypeDto, FieldType>().ReverseMap();
            CreateMap<MemberTypeDto, MemberType>().ReverseMap();
            CreateMap<DraftMemberTypeDto, MemberType>().ReverseMap();

            CreateMap<FormFieldsDto, FormFields>()
                .ForMember(f => f.FieldId, o => o.MapFrom(f => f.FieldId))
                .ForMember(f => f.IsRequired, o => o.MapFrom(f => f.IsRequired))
                .ForMember(f => f.Index, o => o.MapFrom(f => f.Index))
                .ForMember(f => f.IsEditable, o => o.MapFrom(f => f.IsEditable))
                .ForMember(f => f.IsHidden, o => o.MapFrom(f => f.IsAdminFilled));

            CreateMap<CreateUpdateFormDto, Form>().ReverseMap();


            CreateMap<Field, FieldDto>().ReverseMap();
            CreateMap<Field, DraftFieldDto>().ReverseMap();
            CreateMap<CreateFieldDto, Field>();

            CreateMap<UpdateFieldDto, Field>();
            CreateMap<Form, ReturnFormDto>();
            CreateMap<Form, ReturnDraftFormDto>();


            CreateMap<subMemberMembershipDto, Data.Membership.Entities.Membership>().ReverseMap();
            CreateMap<MembershipRequest, SubMemberMembershipRequestDto>().ReverseMap();
            CreateMap<ReturnRequestTypeDto, RequestType>().ReverseMap();
            CreateMap<RequestTypeCategory, ReturnRequestTypesGrouped>().ReverseMap();
            CreateMap<FormUserResponseDto, FormUserResponse>().ReverseMap();
            CreateMap<FieldResponseDto, FieldResponse>().ReverseMap();
            CreateMap<Data.Membership.Entities.Membership, MembershipRequest>()
                .ForMember(request => request.BirthDate, o => o.MapFrom(membership => membership.BirthDate))
                .ForMember(request => request.MembershipId, o => o.MapFrom(membership => membership.Id));

            CreateMap<Form, FormDto>()
      .ForMember(
          dest => dest.Fields,
          opt => opt.MapFrom(src => src.FormFields)
      );
            CreateMap<FormFields, FieldDto>()
       .ForMember(dest => dest.Id,
           opt => opt.MapFrom(src => src.Field.Id))

       .ForMember(dest => dest.Title,
           opt => opt.MapFrom(src => src.Field.Title))

       .ForMember(dest => dest.TitleAr,
           opt => opt.MapFrom(src => src.Field.TitleAr))

       .ForMember(dest => dest.FieldTypeId,
           opt => opt.MapFrom(src => src.Field.FieldTypeId))

       .ForMember(dest => dest.FieldTypeName,
           opt => opt.MapFrom(src => src.Field.FieldType.Name))

       .ForMember(dest => dest.IsRequired,
           opt => opt.MapFrom(src => src.IsRequired))

       .ForMember(dest => dest.IsEditable,
           opt => opt.MapFrom(src => src.IsEditable))
        .ForMember(dest => dest.IsHidden,
           opt => opt.MapFrom(src => src.IsHidden))

       .ForMember(dest => dest.DropdownValuesDto,
           opt => opt.MapFrom(src => src.Field.DropdownValues
               .Select(x => x.Value)
               .ToList()));
            CreateMap<PackageRequestAuthourizations, PackageRequestAuthorizationDto>()
                .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType))
                .ForPath(dest => dest.RequestAuthourizationName,
                    opt => opt.MapFrom(src => src.RequestType.Name));
            // Mapping for RequestType
            CreateMap<RequestType, PackageRequestTypeDto>().ReverseMap();
            CreateMap<PackageMemberTypeDto, MemberType>().ReverseMap();
            CreateMap<PackageFormDto, Form>().ReverseMap();
            CreateMap<Configuration, ConfigurationDto>().ReverseMap();
            CreateMap<ConfigurationField, ConfigurationFieldDto>().ReverseMap();
            CreateMap<GetFormUserResponseDto, FormUserResponse>().ReverseMap();
            CreateMap<GetFieldResponseDto, FieldResponse>().ReverseMap();
            CreateMap<ConfigrationFieldResponseDto, ConfigrationFieldResponse>().ReverseMap();
            CreateMap<PaakageMemberFormDto, PaakageMemberForm>().ReverseMap();
            CreateMap<PackageConfigurationSectionDto, PackageConfigurationSection>().ReverseMap();
            CreateMap<FormUserResponse, MembershipRequest>()
                .AfterMap((src, dest) =>
                {
                    foreach (var fieldResponse in src.FieldsResponse)
                    {
                        var mapField = fieldResponse.Field.MapField;
                        var responseValue = fieldResponse.Response;
                        if (!string.IsNullOrWhiteSpace(mapField))
                        {
                            var property = typeof(MembershipRequest).GetProperty(mapField);
                            if (property is not null && property.CanWrite)
                            {
                                // Convert the response value to the correct type if necessary
                                object? ConvertedValue = ConvertHelper.ConvertToPropertyType(property.PropertyType, responseValue);
                                property.SetValue(dest, ConvertedValue);
                            }
                        }
                    }
                });
            CreateMap<MembershipRequest, Data.Membership.Entities.Membership>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); 


            CreateMap<PackageRequestAuthourizations, MembershipAuthorizationDto>()
                .ForMember(dist => dist.Id, o => o.MapFrom(src => src.RequestType.Id))
                .ForMember(dist => dist.Name, o => o.MapFrom(src => src.RequestType.Name));

            CreateMap<Form, NewRegistrationFormToReturnDto>()
            .ForMember(d => d.User, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.CreationDate, o => o.MapFrom(s => s.CreationDate.HasValue
                ? DateOnly.FromDateTime(s.CreationDate.Value)
                : default(DateOnly?))); // 

            CreateMap<FreezeConfiguration, FreezeConfigurationDto>()
                .ForMember(d => d.UnitName, o => o.MapFrom(src => src.Unit.Name))
                .ReverseMap();
            CreateMap<FreezeConfiguration, FreezeConfigurationSyncDto>()
                .ReverseMap();

            CreateMap<RemovalConfiguration, RemovalConfigurationDto>()
                .ForMember(dist => dist.SubMemberRemovalConfigurationDtos, o => o.MapFrom(src => src.SubMemberRemovalConfigurations))
                .ReverseMap();
            CreateMap<SubMemberRemovalConfiguration, SubMemberRemovalConfigurationDto>()
                .ForMember(dist => dist.MemberTypeName, o => o.MapFrom(src => src.MemberType.Name))
                .ReverseMap();
            CreateMap<SubMemberRemovalConfiguration, SubMemberRemovalConfigurationSyncDto>()
                .ReverseMap();

            CreateMap<RemovalConfiguration, RemovalConfigurationSyncDto>()
                .ForMember(dist => dist.SubMemberRemovalConfigurationSyncDtos, o => o.MapFrom(src => src.SubMemberRemovalConfigurations))
                .ReverseMap();

            CreateMap<RemovalConfiguration, RemovalConfigurationWithoutUnitDto>().ReverseMap();

            CreateMap<CancellationConfiguration, CancellationConfigurationDto>()
                .ForMember(dist => dist.UnitName, o => o.MapFrom(src => src.Unit.Name))
                .ReverseMap();

            CreateMap<CancellationConfiguration, CancellationConfigurationSyncDto>()
                .ReverseMap();

            CreateMap<Unit, AddUnitDto>().ReverseMap();

            CreateMap<Unit, UnitDto>()
                .ForMember(dist => dist.UnitTypeName, o => o.MapFrom(src => src.UnitType.Name))
                .ForMember(dist => dist.UnitName, o => o.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<MembershipRequest, MembershipRequestDto>()
            .ForMember(dest => dest.SubMembers, opt => opt.MapFrom(src => src.SubMembers));


            CreateMap<FormUserResponse, GetFormUserResponseDto>()
                .ForMember(dest => dest.FieldsResponse, opt => opt.MapFrom(src => src.FieldsResponse))
                .ForMember(dest => dest.FormFees, opt => opt.MapFrom(src => src.Form.Fees));

            CreateMap<AdditionalConfiguration, AdditionalConfigurationDto>().ReverseMap();
            CreateMap<AddSubMemberAdditionalConfigurationDto, SubMemberAdditionalConfiguration>()
     .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
     .ReverseMap();

            CreateMap<MemberType, GetPackageMemberTypesDto>()
                .ForMember(dist => dist.MemberId, o => o.MapFrom(src => src.Id))
                .ForMember(dist => dist.MemberName, o => o.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<Unit, FieldUnitsDto>();
            CreateMap<NewMembershipConfiguration, NewMembershipConfigurationDto>().ReverseMap();
            CreateMap<RenewalConfiguration, RenewalConfigurationDto>().ReverseMap();
            CreateMap<RenwalOption, RenwalOptionDto>().ReverseMap();

            CreateMap<PaymentInvoice, PaymentInvoiceDto>();
            CreateMap<PaymentItem, ItemDto>();
            CreateMap<PaymentItem, ItemResultDto>();

            CreateMap<PaymentInvoice, PaymentInvoiceDetailsDto>();

            CreateMap<MembershipRequest, SubMemberDataDto>()
                .ForMember(subMember => subMember.SubMemberDto, o => o.MapFrom(request => request))
                .ForMember(submember => submember.FormUserResponseDto, o => o.MapFrom(request => request.FormUserResponse))
                //.ForMember(submember => submember.DocumentsSubmitted , o => o.MapFrom(request => 
                // request.FormUserResponse.FieldsResponse.Where(response => response.Field.FieldTypeId == 6)
                //.Select(response => response.Response)))
                ;
            CreateMap<MembershipRequest, SubMemberDto>()
                .ForMember(submember => submember.MemberTypeName, o => o.MapFrom(request => request.MemberType.Name));

            CreateMap<FieldResponse, FieldResponseDto>()
                .ForMember(dist => dist.FieldName, o => o.MapFrom(src => src.Field.Title))
                .ForMember(dist => dist.FieldTypeName, o => o.MapFrom(src => src.Field.FieldType.Name));

            CreateMap<FieldResponse, DocumentSubmitted>()
                .ForMember(doc => doc.FieldName, o => o.MapFrom(response => response.Response))
                .ForMember(doc => doc.FieldName, o => o.MapFrom(response => response.Field.Title));

            CreateMap<FreezeConfigurationDto, GetFreezeConfigurationDto>()
                .ReverseMap();
            CreateMap<FreezeConfiguration, GetFreezeConfigurationDto>()
                .ReverseMap();

            CreateMap<NewMembershipConfigurationDto, GetNewMembershipConfigurationDto>()
                .ReverseMap();
            CreateMap<RenewalConfigurationDto, GetRenewalConfigurationDto>()
                .ReverseMap();

            CreateMap<AdditionalConfigurationDto, GetAdditionalConfigurationDto>()
                .ReverseMap();

            CreateMap<CancellationConfigurationDto, GetCancellationConfigurationDto>()
                .ReverseMap();

            CreateMap<RemovalConfigurationDto, GetRemovalConfigurationDto>()
                .ReverseMap();
            CreateMap<MembershipDto, MembershipRequestDto>()
             .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
             .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
             .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.DateOfBirth))
             .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalID))
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
             .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.EndDate))
             .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
             .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality))
             .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Adress))
             .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
             .ForMember(dest => dest.MembershipType, opt => opt.MapFrom(src => src.MembershipType))
             .ForMember(dest => dest.MembershipId, opt => opt.Ignore()) // Ensure this mapping exists
             .ForMember(dest => dest.SubMembers, opt => opt.Ignore())

             .ForMember(dest => dest.PaymentRequestDto, opt => opt.Ignore()); // Ignore SubMembers initially


            CreateMap<Data.Membership.Entities.Membership, MembershipDto>();

            CreateMap<RenewalConfiguration, GetRenewalConfigurationDto>().ReverseMap();
            CreateMap<RenewalConfigurationDto, GetRenewalConfigurationDto>().ReverseMap();
            CreateMap<FormUserResponseDto, ReturnFormUserResponseDto>();



            CreateMap<AddAtheleteFormResponseDto, FormUserResponse>().ReverseMap();

            CreateMap<AddAtheleteFormResponseDto, ReturnAtheleteFormUserResponseDto>()
          .ReverseMap();


            CreateMap<FormUserResponse, CreateAtheleteMembershipRequestForResponseDto>()
             .ReverseMap();
            CreateMap<AddAtheleteFormResponseDto, MembershipRequest>();

            CreateMap<AtheleteFieldResponseDto, FieldResponse>()

                .ReverseMap();
            CreateMap<AtheleteFieldResponseDto, FieldResponseDto>();

            CreateMap<AddAtheleteFormResponseDto, MembershipRequest>();

            CreateMap<PaymentBatch, GetAllFedrationPaymentRequestToReturnDto>()
                .ForMember(dest => dest.PaymentRequestId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NumberOfRequests, opt => opt.MapFrom(src => src.NewPaymentRequests.Count()))
                .ForMember(dest => dest.BatchNumber, opt => opt.MapFrom(src => src.Batch.BatchNumber))
                .ForMember(dest => dest.DateOfSubmission, opt => opt.MapFrom(src => src.CreationDate))
                //.ForMember(dest => dest.TotalAmountDue, opt => opt.MapFrom(src => src.Batch.PackagePrice.HasValue ? src.Batch.PackagePrice.Value * src.NewPaymentRequests.Count : 0))
                .ForMember(dest => dest.NumberOfRequests, opt => opt.MapFrom(src => src.NewPaymentRequests.Count))
                .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.OperationType.ToDisplayName()))
                .ForMember(destinationMember => destinationMember.Branch, opt => opt.MapFrom(src => src.Entity.Area.Name))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus));


            CreateMap<PaymentBatch, GetAllEntityPaymentRequestsToReturnDto>()
               .ForMember(dest => dest.PaymentRequestId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.NumberOfRequests, opt => opt.MapFrom(src => src.NewPaymentRequests.Count()))
               .ForMember(dest => dest.DateOfSubmission, opt => opt.MapFrom(src => src.CreationDate))
               //.ForMember(dest => dest.TotalAmountDue, opt => opt.MapFrom(src => src.Batch.PackagePrice.HasValue ? src.Batch.PackagePrice.Value * src.NewPaymentRequests.Count : 0))
               .ForMember(dest => dest.NumberOfRequests, opt => opt.MapFrom(src => src.NewPaymentRequests.Count))
               .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.OperationType.ToDisplayName()))
               .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus));


            CreateMap<Data.Membership.Entities.Membership, TenantMemberDto>()
                .ForMember(dist => dist.MembershipStatus, o => o.MapFrom(src => src.MembershipStatus))
                .ForMember(dist => dist.Name, o => o.MapFrom(src => src.FirstName + " " + src.LastName))
                .ReverseMap();
            CreateMap<PaymentBatch, GetFedrationPaymentRequestToReturnDto>()
               .ForMember(dest => dest.PaymentRequestId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.NumberOfRequests, opt => opt.MapFrom(src => src.NewPaymentRequests.Count()))
               //.ForMember(dest => dest.BatchNumber, opt => opt.MapFrom(src => src.Batch.BatchNumber))
               .ForMember(dest => dest.DateOfSubmission, opt => opt.MapFrom(src => src.CreationDate))
               .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
               .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.OperationType.ToDisplayName()))
               .ForMember(dest => dest.InvoiceUrl, opt => opt.MapFrom(src => src.InvoiceUrl));
            // .ForMember(dest => dest.TotalAmountDue, opt => opt.MapFrom(src => src.Batch.PackagePrice.HasValue ? src.Batch.PackagePrice.Value * src.NewPaymentRequests.Count : 0))

            //.ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.NewPaymentRequests.Select(npr => string.Concat(npr.MembershipRequest.FirstName, " ", npr.MembershipRequest.LastName))));

            CreateMap<NewPaymentRequest, GetAllEntityPaymentRequestToReturnDto>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.MembershipRequest.FirstName + " " + src.MembershipRequest.LastName))
                .ForMember(dest => dest.BatchNumber, opt =>
                    opt.MapFrom(src => src.Batch.BatchNumber))
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RequestStatus, opt =>
                    opt.MapFrom(src => src.MembershipRequest.Status))
                 .ForMember(dest => dest.RequestType, opt =>
                    opt.MapFrom(src => src.MembershipRequest.Batch.RequestType.Name ?? "N/A"))
                 .ForMember(dest => dest.RequestBranchAdminStatus, opt =>
                    opt.MapFrom(src => src.MembershipRequest.BranchAdminStatus))

                .ForMember(dest => dest.DateOfSubmission, opt =>
                    opt.MapFrom(src => src.MembershipRequest.CreationDate))
                .ForMember(dest => dest.PaymentStatus, opt =>
                    opt.MapFrom(src => src.PaymentStatus))

                 .ForMember(dest => dest.PaymentBranchAdminStatus, opt =>
                    opt.MapFrom(src => src.BranchAdminStatus))
                  .ForMember(dest => dest.BranchName, opt =>
                    opt.MapFrom(src => src.Entity.Area.Name ?? "N/A"))
                .AfterMap((src, dest) =>
                {
                    var fieldResponse = src.MembershipRequest?.FormUserResponse?.FieldsResponse?
                        .FirstOrDefault(fr => fr.Field.MapField == "FileNumber");

                    dest.FileNumber = fieldResponse?.Response ?? "N/A";
                });
            CreateMap<Area, GetAreasDto>()
                .ForMember(dto => dto.NumberOfEntities, o => o.MapFrom(area => area.Entities.Count))
                .ReverseMap();
            CreateMap<BankAccount, BankAccountDto>().ReverseMap();
            CreateMap<Area, AreaWithAccountsDto>().ReverseMap();
            CreateMap<AddAreaDto, Area>();
            CreateMap<DropdownValue, AreaPricing>()
                .ForMember(pricing => pricing.BeltId, o => o.MapFrom(value => value.Id))
                .ForMember(pricing => pricing.BeltName, o => o.MapFrom(value => value.Value))
                .ForMember(pricing => pricing.Id, o => o.Ignore());
            CreateMap<AreaPricingDto, AreaPricing>().ReverseMap();
            CreateMap<UpdateAreaPricingDto, AreaPricing>().ReverseMap();
            CreateMap<AddAreaConfigurationDto, AreaConfiguration>().ReverseMap();
            CreateMap<AreaConfiguration, AreaConfigurationDto>()
                .ForMember(dto => dto.FormName, o => o.MapFrom(config => config.Form.Title));
            CreateMap<UpdateAreaConfigurationDto, AreaConfiguration>().ReverseMap();

            CreateMap<Tenant, AddEntityDto>()
             .ReverseMap();
            CreateMap<Tenant, UpdateEntityDto>().ReverseMap();
            CreateMap<Tenant, EntityDto>()
        .ForMember(dest => dest.EntityTypeName, opt => opt.MapFrom(src => src.FedrationEntityType != null ? src.FedrationEntityType.Name : null))
        .ForMember(dest => dest.FedrationBranchName, opt => opt.MapFrom(src => src.Area != null ? src.Area.Name : null))
        .ForMember(dest => dest.FedrationBranchId, opt => opt.MapFrom(src => src.AreaId != null ? src.AreaId : null))
        .ReverseMap();


            CreateMap<EntityDto, Tenant>()
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.FedrationBranchId));
            CreateMap<Tenant, GetBranchEntityDto>()
                .ForMember(dto => dto.EntityTypeId, o => o.MapFrom(src => src.FedrationEntityTypeId))
                .ForMember(dto => dto.EntityTypeName, o => o.MapFrom(src => src.FedrationEntityType.Name));

            CreateMap<DropdownValue, DropdownDto>();

            CreateMap<MembershipRequest, GetEntityMembershipRequestDto>();
            CreateMap<AddBulkEntityDto, Tenant>();

            CreateMap<Data.Membership.Entities.Membership, FederationMemberDto>()

                .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src => src.Tenant.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FileNumber))
                .ForMember(dest => dest.BeltName, opt => opt.MapFrom(src => src.BeltName))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Tenant.Area.Name)).ReverseMap();


            CreateMap<PreRegisteredUser, PreRegisteredUserToReturnDto>();

            CreateMap<HistoricalTournament, HistoricalTournamentDto>()
                .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src =>
                    src.Results
                        .Where(r => r.MembershipId.HasValue)
                        .Select(r => r.MembershipId.Value)
                        .Distinct()
                        .Count()));

            CreateMap<CreateHistoricalTournamentDto, HistoricalTournament>();
            CreateMap<HistoricalTournamentUpdateDto, HistoricalTournament>();
            CreateMap<TournamentResult, TournamentResultDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Membership.FirstName))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Membership.UserPhoto))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Membership.Gender.ToString()))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.Membership.BirthDate))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src =>
                src.Time.HasValue
            ? $"{(int)src.Time.Value.TotalHours:D2}:{src.Time.Value.Minutes:D2}:{src.Time.Value.Seconds:D2}.{(src.Time.Value.Milliseconds / 10):D2}"
            : null))
            .ForMember(dest => dest.RaceName, opt => opt.MapFrom(src => src.RaceName))
            .ForMember(dest => dest.Ranking, opt => opt.MapFrom(src => src.Ranking));
            CreateMap<Data.Membership.Entities.Membership, CreateMembershipDto>()
                .ReverseMap();

            CreateMap<UpdateMembershipDto, Data.Membership.Entities.Membership>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) =>
                        srcMember != null &&
                        !(srcMember is DateTime dt && dt == default) &&
                        !(srcMember is int i && i == default)
                    )
                );
            CreateMap<Data.Membership.Entities.Membership, GetAllUserMemberships>()
            .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant.Name))
            .ForMember(dest => dest.TenantImage, opt => opt.MapFrom(src => src.Tenant.Image))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package.Name))
             .AfterMap((src, dest, ctx) =>
             {
                 if (src.PackageId.HasValue && src.Package != null && src.Package.Branches != null)
                 {
                     dest.Branches = src.Package.Branches
                                             .Select(pb => pb.Branch.Name)
                                             .ToList();
                 }
             });
            CreateMap<MembershipRequest, GetAllUserMembershipRequests>()
           .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant.Name))
           .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => (src.FirstName + " " + src.LastName).Trim()))
           .ForMember(dest => dest.TenantImage, opt => opt.MapFrom(src => src.Tenant.Image));

            // CreateMap<MembershipRequest, MembershipRequestWithFormDataDto>()
            // .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.Name : null))
            // //.ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package != null ? src.Package.Name : null))
            // .ForMember(dest => dest.FormFees, opt => opt.MapFrom(src =>
            //     src.FormUserResponse != null && src.FormUserResponse.Form != null
            //         ? src.FormUserResponse.Form.Fees
            //         : (double?)null))
            // .ForMember(dest => dest.FormUserResponse, opt => opt.MapFrom(src => src.FormUserResponse))
            // .ForMember(dest => dest.SubMembers, opt => opt.MapFrom(src => src.SubMembers))
            // .ForMember(dest => dest.Package, opt => opt.MapFrom(src =>
            //     src.Package == null
            //         ? null
            //         : new PackageSummaryDto
            //         {
            //             Id = src.Package.Id,
            //             Name = src.Package.Name,
            //             Image = src.Package.Image,
            //             Price = src.Package.Price,
            //             Duration = src.Package.Duration,
            //             BranchCount = src.MembershipRequestAccesses != null
            //                 ? src.MembershipRequestAccesses.Select(a => a.BranchId).Distinct().Count()
            //                 : 0,
            //             FacilityCount = src.MembershipRequestAccesses != null
            //                 ? src.MembershipRequestAccesses.Select(a => a.FacilityId).Distinct().Count()
            //                 : 0,
            //             VenueCount = src.MembershipRequestAccesses != null
            //                 ? src.MembershipRequestAccesses.Select(a => a.VenueId).Distinct().Count()
            //                 : 0
            //         }
            // ))
            //.ForMember(dest => dest.AccessTree, opt => opt.MapFrom(src => new MembershipRequestAccessTreeDto
            //{
            //    SelectedBranches = src.MembershipRequestAccesses
            // .Select(a => a.BranchId)
            // .Distinct()
            // .ToList(),

            //    SelectedFacilities = src.MembershipRequestAccesses
            // .Where(a => a.FacilityId != 0)
            // .Select(a => a.FacilityId)
            // .Distinct()
            // .ToList(),

            //    SelectedVenues = src.MembershipRequestAccesses
            // .Where(a => a.VenueId != 0)
            // .Select(a => a.VenueId)
            // .Distinct()
            // .ToList()
            //}));
            CreateMap<AddPackageWithSubPackagesDto, Package>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Image, opt => opt.Ignore())
                .ForMember(d => d.Venues, opt => opt.Ignore())
                .ForMember(d => d.Branches, opt => opt.Ignore())
                .ForMember(d => d.Facilities, opt => opt.Ignore())
                .ForMember(d => d.PackageProducts, opt => opt.Ignore())
                .ForMember(d => d.SubPackages, opt => opt.Ignore());
            CreateMap<Package, DTOs.Membership.PackageDtos.PackageDto>();
            CreateMap<SubPackageDto, Package>();

            CreateMap<SubMemberCategory, SubMemberCategoryDto>().ReverseMap();

            CreateMap<PackageAuthourizations, DTOs.Membership.PackageDtos.PackageAuthourizationsDto>().ReverseMap();
            CreateMap<SubMemberCategory, subMemberMembershipDto>();

            CreateMap<SubMemberCategoryDto, SubMemberCategory>();

            CreateMap<FormUserResponse, GetParentFormUserResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FieldsResponse, opt => opt.MapFrom(src => src.FieldsResponse));

            CreateMap<FieldResponse, ParentFormFieldResponseDto>()
            .ForMember(dest => dest.FieldId,
                opt => opt.MapFrom(src => src.Field.Id))
            .ForMember(dest => dest.FieldName,
                opt => opt.MapFrom(src => src.Field.Title))
            .ForMember(dest => dest.Response,
                opt => opt.MapFrom(src => src.Response))
            .ForMember(dest => dest.Index,
                opt => opt.MapFrom(src =>
                    src.Field.FormFields
                        .Where(ff => ff.FormId == src.FormUserResponse.FormId
                                     && ff.FieldId == src.FieldId)
                        .Select(ff => ff.Index)
                .FirstOrDefault()
                    )
                )
            .ForMember(dest => dest.IsHidden,
                opt => opt.MapFrom(src =>
                    src.Field.FormFields
                        .Where(ff => ff.FormId == src.FormUserResponse.FormId
                                     && ff.FieldId == src.FieldId)
                        .Select(ff => ff.IsHidden)
                .FirstOrDefault()
                    )
                );


        }
        private string ResolveImage(string image)
        {
            return DocumentSetting.GetPreSignedUrlAsync(image).GetAwaiter().GetResult();
        }
    }
}