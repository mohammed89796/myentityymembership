using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.ParentApplicationFormDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.ParentApplicationFormService
{
    public interface IParentApplicationFormService
    {
        Task<Response<string>> CreateParentApplicationFormAsync(CreateParentApplicationFormDto dto);
        Task<Response<PaginationToReturnDto<ParentApplicationFormDto>>> GetParentApplicationFormsAsync(ParentApplicationFormFilterDto filterDto);
        Task<Response<ParentApplicationFormDto>> GetParentApplicationFormByIdAsync(int id, int? tenantId);
        Task<Response<string>> UpdateParentApplicationFormAsync(UpdateParentApplicationFormDto dto);
        Task<Response<string>> DeleteParentApplicationFormAsync(int id, int? tenantId);

    }
}
