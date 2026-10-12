using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.TenantDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.TenantService
{
    public interface ITenantService
    {
        Task<Response<AddTenantDto>> AddTenant(AddTenantDto addTenantDto);
        Task<Response<List<AddTenantDto>>> GetAllTenants(string? searchName);
        Task<Response<TenantDto>> GetTenantByIdAsync(int tenantId);
        Task<Response<UpdateTenantDto>> UpdateTenant(UpdateTenantDto updateTenantDto);
        Task<Response<IReadOnlyList<AddTenantDto>>> GetTenantsByListOfIdsAsync(List<int> tenantIds);
        Task<Response<List<AddTenantDto>>> GetAllFederationss(string? searchName);
        Task<Response<TenantDto>> GetFedByIdAsync(int tenantId);
        Task<Response<string>> CheckTanantTypeById(int tenantId);
        Task<Response<List<AddTenantDto>>> GetFederationEntities(int Id);
        Task<Response<TenantDetailsDto>> GetTenantDetails(int tenantId);
        Task<Response<List<GetBranchEntityDto>>> GetEntitiesByArea(GetEntitiesByAreaParametersDto parameters);
        Task<Response<GetAreaStatisticsDto>> GetAreaStatistics(int AreaId);

        Task<Repositories.Common.Response<PaginationToReturnDto<SuperEntityToReturnDto>>> GetAllSuperEntitiesAsync(SuperEntityDto dto);

        Task<Repositories.Common.Response<List<object>>> GetEntityTypeAsync(string? searchTerm);

        Task<Repositories.Common.Response<string>> ReachOutToTenantAsync(ReachOutToTenant dto, string userName, string userEmail);
    }
}
