using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.FacilityServicesService;

namespace MyEntityMembershipService.Controllers.V1.Entity.Facility
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class FacilityServicesController : ControllerBase
    {
        private readonly IFacilityServicesService _facilityServicesService;

        public FacilityServicesController(IFacilityServicesService facilityServicesService)
        {
            _facilityServicesService = facilityServicesService;
        }

        [HttpGet("CreateFacilityService")]
        public async Task<IResult> CreateFacilityService(string typename)

            => (await _facilityServicesService.CreateFacilityService(typename)).GetResponse();

        [HttpGet("GetFacilityServicesDropDown")]
        public async Task<IResult> GetFacilityServicesDropDown()

            => (await _facilityServicesService.GetFacilityServicesDropDown()).GetResponse();

    }
}
