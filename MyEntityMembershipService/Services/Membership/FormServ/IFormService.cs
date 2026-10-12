using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.ConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.Repositories.Common;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Services.Membership.FormServ
{
    public interface IFormService
    {

        Task<Response<CreateUpdateFormDto>> CreateUpdateForm(CreateUpdateFormDto formDto);
        

        Task<Response<ReturnFormDto>> GetForm(int id, int? packageId, int? memberTypeId);
        Task<Response<List<ReturnFormDto>>> GetFormsCreatedBy(string userId, int requestTypeId, string? title,int? tenantId);
        Task<Response<List<ReturnRequestTypesGrouped>>> GetRequestTypesGroupedByCategories(int? tenantId);
        Task<Response<ReturnFormDto>> DeleteForm(int id);
        Task<Response<ReturnFormUserResponseDto>> AddFormUserResponse(FormUserResponseDto formUserResponseDto);
        Task<Response<string>> ExecuteLogicForRequestType(ExecuteLogicForRequestTypeDto model);
        Task<Response<List<FormDto>>> GetFormsByMemberTypeAndRequestType(int memberTypeId, int requestTypeId);

        Task<Response<List<RequestTypeFieldsGroupedByCategoryDto>>> GetRequestTypeFields(int requestTypeId);
        Task<Response<string>> UploadFile(IFormFile file);

        Task<Response<Dictionary<string, List<FormDto>>>> GetFormsForEachSubMemberAsync(int packageId, int requestTypeId);
        Task<Response<List<ConfigurationResponseDto>>> GetPackageDetailsByRequestTypeAndPackageID(int requestTypeId, int packageID);
        Task<Response<GetFreezCardDto>> GetFreezCard(int MembershipId);
        Task<Response<FreezAdditionalPayment>> CheckExceedWithAdditionalPayment(FormUserResponseDto formUserResponseDto);
        Task<Response<bool>> CheckCanSelectPackage(int packageId, string userId);

        Task<Response<IReadOnlyList<NewRegistrationFormToReturnDto>>> GetNewRegistrationFormsByRequestTypeId();

        Task<Response<NewRegistrationFormToReturnDto>> GetNewRegistrationFormsById(int Id);

        Task<Response<ReturnDraftFormDto>> GetDraftForm(int id, int? packageId);
        Task<Response<FormDto>> GetFormsForSubMemberAsync(int packageId, int requestTypeId, int? memberTypeId);

        Task<Response<PaginationToReturnDto<FormResponseDto>>> GetAllFormsByTenantIdAsync(int requestTypeId , FormResponseSearchDto dto);
    }
}
