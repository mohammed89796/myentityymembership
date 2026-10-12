

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.FacilityTypesService
{
    public interface IFacilityTypesService
    {
        Task<Response<bool>> CreateFacilityType(string typeName);
        Task<Response<List<FacilityType>>> GetFacilityTypesDropDown();

    }
}
