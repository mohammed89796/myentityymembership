using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.DTOs.Membership.FederationEntitiesDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.EntityService
{
    public interface IEntityService
    {
        Task<Response<EntityDto>> CreateFederationEntity(EntityDto dto);
        Task<Response<PagedResult<EntityDto>>> GetFederationEntitiesAsync(GetFederationEntitiesParameterDto parameters);
        Task<Response<EntityDto>> EditFederationEntity(UpdateEntityDto dto);
        Task<Response<string>> DeleteFederationEntity(int id);
        Task<Response<EntityDto>> GetFederationEntityByIdAsync(int id);
        Task<Response<EntityCountsDto>> GetEntityCountsAsync(int tenantId);
    }
}
