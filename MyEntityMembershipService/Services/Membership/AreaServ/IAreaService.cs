using MyEntityMembershipService.DTOs.Membership.AreaDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.AreaServ
{
    public interface IAreaService
    {
        Task<Response<List<GetAreasDto>>> GetAllAreas(int FederationId, string? SearchBy, int PageIndex, int PageSize);
        Task<Response<AreaWithAccountsDto>> GetAreaWithBankAccounts(int AreaId);
        Task<Response<bool>> AddArea(AddAreaDto AreaDto);
        Task<Response<bool>> UpdateArea(AreaWithAccountsDto areaDto);
        Task<Response<bool>> UpdateAreaPricing(UpdateAreaPricingDto areaPricingDto);
        Task<Response<List<AreaPricingDto>>> GetAllAreaPricing(GetAllAreaPricingParametersDto parameters);
        Task<Response<bool>> AddAreaConfiguration(AddAreaConfigurationDto areaConfigurationDto);
        Task<Response<bool>> UpdateAreaConfiguration(UpdateAreaConfigurationDto areaConfigurationDto);
        Task<Response<List<AreaConfigurationDto>>> GetAreaConfigurations(int AreaId);
        Task<Response<bool>> DeleteAreaConfiguration(int Id);
        Task<Response<bool>> AddAreaBankAccount(AddAreaBankAccountDto parameters);
        Task<Response<List<GetAreasDto>>> GetAllFederationAreas(int FederationId, string userId);
        Task<Response<List<GetBranchEntityDto>>> GetEntitiesByArea(GetEntitiesByAreaParametersDto parameters);
        Task<Response<List<GetAreasDto>>> GetAllFederationAreasByEntityId(int entityId);
        Task<Response<List<GetBranchEntityDto>>> GetEntitiesByAreaWithoutPagination(int areaId);
    }
}
