using MyEntityMembershipService.DTOs.Entity.SystemDtos.CountryDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.System.CountryService
{
    public interface ICountryService
    {
        public Task<Response<bool>> CreateCountry(string countryName);
        public Task<Response<List<CountryViewModel>>> GetAllCountries();

    }
}