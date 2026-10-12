using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.FacilityTypesService;


namespace MyEntityMembershipService.Controllers.V1.Entity.Facility
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class FacilityTypesController : ControllerBase
    {
        private readonly IFacilityTypesService _facilityTypeService;

        public FacilityTypesController(IFacilityTypesService facilityTypeService)
        {
            _facilityTypeService = facilityTypeService;
        }

        [HttpGet("CreateFacilityType")]
        public async Task<IResult> CreateFacilityType(string typename)

            => (await _facilityTypeService.CreateFacilityType(typename)).GetResponse();

        [HttpGet("GetFacilityTypesDropDown")]
        public async Task<IResult> GetFacilityTypesDropDown()

            => (await _facilityTypeService.GetFacilityTypesDropDown()).GetResponse();

    }
}
