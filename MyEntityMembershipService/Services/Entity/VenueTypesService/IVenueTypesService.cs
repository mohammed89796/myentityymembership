

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.VenueTypesService
{
    public interface IVenueTypesService
    {
        Task<Response<bool>> CreateVenueType(string typeName);
        Task<Response<List<VenueTypes>>> GetVenueTypesDropDown();

    }
}
