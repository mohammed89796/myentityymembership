using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.AmenitiesService;

namespace MyEntityMembershipService.Controllers.V1.Entity.Facility
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class AmenitiesController : ControllerBase
    {
        private readonly IAmenitiesService _amenitiesService;

        public AmenitiesController(IAmenitiesService amenitiesService)
        {
            _amenitiesService = amenitiesService;
        }

        [HttpGet("CreateAmenity")]
        public async Task<IResult> CreateAmenity(string typename)

            => (await _amenitiesService.CreateAmenity(typename)).GetResponse();

        [HttpGet("GetAmenitiesDropDown")]
        public async Task<IResult> GetAmenitiesDropDown()

            => (await _amenitiesService.GetAmenitiesDropDown()).GetResponse();

    }
}
