using MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.RemovalConfigurationDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.RemovalConfigurationService
{
    public interface IRemovalConfigurationService
    {
        Task<Response<RemovalConfigurationDto>> GetByPackageId(int packageId);
        Task<Response<RemovalConfigurationSyncDto>> SyncRemovalConfiguration(RemovalConfigurationSyncDto removalConfigurationDto);
        Task<Response<bool>> DeleteAsync(int id);
        Task<Response<GetRemovalConfigurationDto>> GetRemovalFeeByPackageIdAndSubMembersIdAsync(int packageId, int? subMemberId = null);
    }
}
