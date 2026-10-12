using MyEntityMembershipService.DTOs.Entity.EntityAdminDtos;
using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.EntityAdminService
{
    public interface IEntityAdminService
    {
        Task<Response<bool>> AddEntityAdmin(AddEntityAdminDto entityAdminDto);
        Task<Response<UpdateEntityAdminDto>> UpdateEntityAdmin(UpdateEntityAdminDto entityAdminDto);
        Task<Response<List<GetEntityAdminDto>>> GetEntityAdmins(GetEntityAdminsParametersDto parameters);
        Task<Response<List<TenantCoachAdminDto>>> GetTenantCoachAdmins(int tenantId, GetTenantCoachAdminsParametersDto parameters);
        Task<Response<List<GetEntityAdminDto>>> GetFederationEntitiesAdmins(GetFederationEntitiesAdminsParametersDto parameters);

        Task<Response<TenantAdminDto>> GetTenantAdminByUserId(string userId);
        Task<Response<GetEntityAdminDto>> UpdateEntityAdminStatus(string UserId);

        Task<Response<string>> DeactiveEntityAdmin(string userId); 
    }
}
