using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.VenueTypesService;

namespace MyEntityMembershipService.Controllers.V1.Entity.Venue
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class VenueTypesController : ControllerBase
    {
        private readonly IVenueTypesService _VenueTypeservice;

        public VenueTypesController(IVenueTypesService VenueTypeservice)
        {
            _VenueTypeservice = VenueTypeservice;
        }

        [HttpGet("CreatVenueType")]
        public async Task<IResult> CreateVenueType(string typename)

            => (await _VenueTypeservice.CreateVenueType(typename)).GetResponse();

        [HttpGet("GetVenueTypesDropDown")]
        public async Task<IResult> GetVenueTypesDropDown()

            => (await _VenueTypeservice.GetVenueTypesDropDown()).GetResponse();

    }
}
