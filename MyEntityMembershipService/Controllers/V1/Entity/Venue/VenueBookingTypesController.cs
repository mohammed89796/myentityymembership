using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.VenueBookingTypes;

namespace MyEntityMembershipService.Controllers.V1.Entity.Venue
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class VenueBookingTypesController : ControllerBase
    {
        private readonly IVenueBookingTypesService _VenueBookingTypeservice;

        public VenueBookingTypesController(IVenueBookingTypesService VenueBookingTypeservice)
        {
            _VenueBookingTypeservice = VenueBookingTypeservice;
        }

        [HttpPost("CreateVenueBookingType")]
        public async Task<IResult> CreateVenueType([FromForm] string name, string description, IFormFile Icon)

            => (await _VenueBookingTypeservice.CreateVenueBookingTypes(name, description, Icon)).GetResponse();

        [HttpGet("GetVenueBookingTypesAsync")]
        public async Task<IResult> GetVenueBookingTypesAsync()

            => (await _VenueBookingTypeservice.GetVenueBookingTypesAsync()).GetResponse();

    }
}
