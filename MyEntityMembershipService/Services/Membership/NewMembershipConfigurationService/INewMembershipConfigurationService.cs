using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.DTOs.Membership.NewMembershipConfigurationDtos;
using MyEntityMembershipService.Repositories.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Services.Membership.NewMembershipConfigurationService
{
    public interface INewMembershipConfigurationService
    {
      
        Task<Response<List<NewMembershipConfigurationDto>>> GetAllConfigurations();
       
        Task<Response<NewMembershipConfigurationDto>> GetConfigurationById(int id);

        Task<Response<NewMembershipConfigurationDto>> GetConfigurationByPackageId(int packageId);

        Task<Response<NewMembershipConfigurationDto>> CreateConfiguration(NewMembershipConfigurationDto dto);
      
        Task<Response<NewMembershipConfigurationDto>> UpdateConfiguration(int id, NewMembershipConfigurationDto dto);
      
        Task<Response<bool>> DeleteConfiguration(int id);

        Task<Response<NewMembershipConfigurationDto>> SyncConfiguration(int packageId, NewMembershipConfigurationDto dto);
    }
}
