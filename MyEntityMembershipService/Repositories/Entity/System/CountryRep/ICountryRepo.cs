
using MyEntityMembershipService.DTOs.Entity.SystemDtos.CountryDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.System.CountryRep
{
    public interface ICountryRepo : IGenericRepository<Country>
    {
        public Task<List<CountryViewModel>> GetCountries();

    }
}
