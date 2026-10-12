

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.CountryCode
{
    public interface ICountryCodesService
    {
        Task<Response<bool>> CreateCountryCode(string code, IFormFile Flag);
        Task<Response<List<CountryCodes>>> GetCountryCodesDropDown();

    }
}
