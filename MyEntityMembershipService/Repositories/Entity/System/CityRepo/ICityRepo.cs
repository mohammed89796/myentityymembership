
using MyEntityMembershipService.DTOs.Entity.SystemDtos.CityRepo;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.System.CityRepo
{
    public interface ICityRepo : IGenericRepository<City>
    {
        public Task<List<CityViewModel>> GetCities(int stateID);

    }
}
