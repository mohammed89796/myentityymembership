

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.AmenitiesService
{
    public interface IAmenitiesService
    {
        Task<Response<bool>> CreateAmenity(string name);
        Task<Response<List<Amenities>>> GetAmenitiesDropDown();

    }
}
