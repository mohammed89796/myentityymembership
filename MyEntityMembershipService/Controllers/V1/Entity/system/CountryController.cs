using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.System.CountryService;

namespace MyEntityMembershipService.Controllers.V1.Entity.system
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class CountryController : ControllerBase
    {
        private readonly ICountryService countryService;

        public CountryController(ICountryService countryService)
        {
            this.countryService = countryService;
        }

        [HttpGet("CreateCountry")]
        public async Task<IResult> CreateCountry(string countryName)
        {
            var result = await countryService.CreateCountry(countryName);
            return result.GetResponse();
        }
        [HttpGet("GetAllCountries")]
        public async Task<IResult> GetAllCountries()
        {
            var result = await countryService.GetAllCountries();
            return result.GetResponse();
        }

       
    }
}
