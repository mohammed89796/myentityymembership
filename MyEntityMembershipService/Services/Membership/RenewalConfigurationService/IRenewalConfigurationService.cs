using MyEntityMembershipService.DTOs.Membership.RenewalConfigurationDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.RenewalConfigurationService
{
    public interface IRenewalConfigurationService
    {
        Task<Response<List<RenewalConfigurationDto>>> GetAllConfigurations();
        Task<Response<RenewalConfigurationDto>> GetConfigurationById(int id);
        Task<Response<RenewalConfigurationDto>> GetConfigurationByPackageId(int packageId);
        Task<Response<RenewalConfigurationDto>> CreateConfiguration(RenewalConfigurationDto dto);
        Task<Response<RenewalConfigurationDto>> UpdateConfiguration(int id, RenewalConfigurationDto dto);
        Task<Response<bool>> DeleteConfiguration(int id);
        Task<Response<RenewalConfigurationDto>> SyncConfiguration(int packageId, RenewalConfigurationDto dto);
    }
}
