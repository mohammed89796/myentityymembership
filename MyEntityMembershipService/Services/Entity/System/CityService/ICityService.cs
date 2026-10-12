using MyEntityMembershipService.DTOs.Entity.SystemDtos.CityRepo;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.System.CityService
{
    public interface ICityService
    {
        public Task<Response<bool>> CreateCity(int stateID, string cityName);
        public Task<Response<List<CityViewModel>>> GetAllCity(int stateID);

    }
}