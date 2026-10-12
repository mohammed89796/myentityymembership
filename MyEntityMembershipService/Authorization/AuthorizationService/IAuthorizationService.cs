using MyEntityMembershipService.Authorization.Permissions.Dto;

namespace MyEntityMembershipService.Authorization.AuthorizationService
{
    public interface IAuthorizationService
    {
        Task RegisterServiceAsync(ServicePermissionsDto servicePermissionsDto);
        Task<bool> IsAuthorizedAsync(string userId, string permissionCode);
        Task<bool> IsAuthAsync(AuthorizationRequestDto dto);
    }
}
