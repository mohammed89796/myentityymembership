
using MyEntityMembershipService.DTOs.Membership.FreezeConfigurtionDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.FreezeConfigurationService
{
    public interface IFreezeConfigurationService
    {
        Task<Response<List<FreezeConfigurationDto>>> GetAllAsync();
        Task<Response<FreezeConfigurationDto>> GetByIdAsync(int id);
        Task<Response<FreezeConfigurationDto>> AddAsync(FreezeConfigurationDto dto);
        Task<Response<FreezeConfigurationDto>> UpdateAsync(FreezeConfigurationDto dto);
        Task<Response<bool>> DeleteAsync(int id);
        Task<Response<FreezeConfigurationDto>> GetByPackageId(int packageId);
        Task<Response<FreezeConfigurationSyncDto>> SyncFreezeConfiguration(FreezeConfigurationSyncDto freezeConfigurationDto);
    }
}
