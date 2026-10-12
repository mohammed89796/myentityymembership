using MyEntityMembershipService.DTOs.Membership.ConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.Repositories.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Services.Membership.ConfigrationFieldResponseService

{
    public interface IConfigrationFieldResponseService
    {
        Task<Response<List<ConfigrationFieldResponseDto>>> AddConfigrationFieldResponses(List<ConfigrationFieldResponseDto> dtos);
        Task<Response<ConfigrationFieldResponseDto>> UpdateConfigrationFieldResponse(ConfigrationFieldResponseDto dto);
        Task<Response<bool>> DeleteConfigrationFieldResponse(int id);
        Task<Response<List<ConfigrationFieldResponseDto>>> SyncConfigrationFieldResponses(int packageId, List<ConfigrationFieldResponseDto> newDtos);
        Task<Response<List<ConfigrationFieldResponseDto>>> GetAllConfigrationFieldResponses();
        Task<Response<ConfigrationFieldResponseDto>> GetConfigrationFieldResponseById(int id);
        Task<Response<List<IConfigurationResponseBase>>> GetConfigrationFieldResponse(int responseTypeID, int packageID);
    }

}
