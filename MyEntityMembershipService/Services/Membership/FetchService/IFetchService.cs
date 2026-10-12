using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.TempRegisterResponseDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.FetchService
{
    public interface IFetchService
    {
        Task<Tenant> GetTenantEntityAsync(int entityId);
        Task<TenantAdmin> GetTenantAdmin(string userId);
        Task<Response<string>> TempRegisterUser(TempRegisterRequestDto tempRegisterRequestDto);
        Task<Response<List<TempRegisterResponseDto>>> BulkRegisterUsersAsync(List<TempRegisterRequestDto> users);
        Task<Response<UpdateTempUserRequestDto>> UpdateempRegisterUser(UpdateTempUserRequestDto tempRegisterRequestDto);
        Task<Response<string>> DeleteUsers(List<string> users);

        Task<Response<bool>> EntityBlockedAsync(int entityId);

        Task<Response<List<int>>> GetUserBlockedEntitiesAsync();


    }
}
