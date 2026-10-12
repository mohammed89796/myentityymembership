using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.System.CityService;

namespace MyEntityMembershipService.Controllers.V1.Entity.system
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class CityController : ControllerBase
    {
        private readonly ICityService cityService;

        public CityController(ICityService cityService)
        {
            this.cityService = cityService;
        }

        [HttpGet("CreateCity")]
        public async Task<IResult> CreateCity(int stateID, string cityName)
        {
            var result = await cityService.CreateCity(stateID, cityName);
            return result.GetResponse();
        }
        [HttpGet("GetAllCities")]
        public async Task<IResult> GetAllCities(int stateID)
        {
            var result = await cityService.GetAllCity(stateID);
            return result.GetResponse();
        }

       
    }
}
