using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.VenueConfigurationService;

namespace MyEntityMembershipService.Controllers.V1.Entity.Venue
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class VenueConfigurationController : ControllerBase
    {
        private readonly IVenueConfigurationService _venueConfigurationService;

        public VenueConfigurationController(IVenueConfigurationService venueConfigurationservice)
        {
            _venueConfigurationService = venueConfigurationservice;
        }

        [HttpPost("CreateVenueConfiguration")]
        public async Task<IResult> CreateVenueConfiguration(AddVenueConfigurationDto request)

            => (await _venueConfigurationService.CreateNewVenueConfigurationAsync(request)).GetResponse();
        [HttpPost("GetVenueConfiguration")]
        public async Task<IResult> GetVenueConfiguration(int venueID)

            => (await _venueConfigurationService.GetVenueConfiguration(venueID)).GetResponse();

    }
}
