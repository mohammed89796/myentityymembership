using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.CountryCode;

namespace MyEntityMembershipService.Controllers.V1.Entity.system
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class CountryCodesController : ControllerBase
    {
        private readonly ICountryCodesService _countryCodeservice;

        public CountryCodesController(ICountryCodesService countryCodeservice)
        {
            _countryCodeservice = countryCodeservice;
        }

        [HttpPost("CreateCountryCode")]
        public async Task<IResult> CreateCountryCode(string code, IFormFile flag)

            => (await _countryCodeservice.CreateCountryCode(code, flag)).GetResponse();

        [HttpGet("GetCountryCodesDropDown")]
        public async Task<IResult> GetCountryCodesDropDown()

            => (await _countryCodeservice.GetCountryCodesDropDown()).GetResponse();

    }
}
