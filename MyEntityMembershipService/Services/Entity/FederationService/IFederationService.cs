
using MyEntityMembershipService.DTOs.Entity.FederationDtos;
using MyEntityMembershipService.DTOs.Entity.TenantDtos;
using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.FederationService
{
    public interface IFederationService
    {
        Task<Response<FederationEntityDto>> CreateFederationEntity(AddFederationEntityDto dto);
        Task<Response<PagedResult<FederationEntityDto>>> GetFederationEntitiesAsync(int tenentId, int? pageNumber = null, int? pageSize = null);
        Task<Response<FederationEntityDto>> EditFederationEntity(UpdateFederationDto dto);
        Task<Response<string>> DeleteFederationEntity(int id);
        Task<Response<FederationEntityDto>> GetFederationEntityByIdAsync(int id);
        Task<Response<FederationDto>> CreateFederation(AddFederationDto dto);
    }
}
