using MyEntityMembershipService.DTOs.Membership.AdditionalConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.AdditionalConfigurationServ
{
    public interface IAdditionalConfigurationService
    {
        Task<Response<List<AdditionalConfigurationDto>>> GetAllAdditionalConfigurations();
        Task<Response<AdditionalConfigurationDto>> GetAdditionalConfigurationById(int id);
        Task<Response<AdditionalConfigurationDto>> CreateAdditionalConfiguration(AdditionalConfigurationDto dto);
        Task<Response<AdditionalConfigurationDto>> UpdateAdditionalConfiguration(int id, AdditionalConfigurationDto dto);
        Task<Response<bool>> DeleteAdditionalConfiguration(int id);
        Task<Response<AdditionalConfigurationDto>> SyncAdditionalConfiguration(int packageId, AdditionalConfigurationDto dto);

        Task<Response<AdditionalConfigurationDto>> GetAdditionalConfigurationByPackageId(int packageId);
        //Task<Response<GetAdditionalConfigurationDto>> GetAdditionFeeByPackageIdAndSubMembersIdAsync(int packageId, int? subMemberId = null);
        Task<GetAdditionalConfigurationDto> GetAdditionFeeByPackageIdAndSubMembersIdAsync(int packageId, int? subMemberId = null);

    }
}
