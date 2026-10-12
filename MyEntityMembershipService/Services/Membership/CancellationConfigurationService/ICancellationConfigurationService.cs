using MyEntityMembershipService.DTOs.Membership.CancellationConfigurationDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.CancellationConfigurationService
{
    public interface ICancellationConfigurationService
    {
        Task<Response<CancellationConfigurationDto>> GetByPackageIdAsync(int packageId);
        Task<Response<CancellationConfigurationSyncDto>> Sync(CancellationConfigurationSyncDto dto);
        Task<Response<bool>> DeleteAsync(int packageId);
    }
}
