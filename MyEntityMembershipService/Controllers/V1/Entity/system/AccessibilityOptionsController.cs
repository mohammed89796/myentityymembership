using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.AccessibilityOptionsService;

namespace MyEntityMembershipService.Controllers.V1.Entity.system
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]

    public class AccessibilityOptionsController : ControllerBase
    {
        private readonly IAccessibilityOptionsService _facilityServiceService;

        public AccessibilityOptionsController(IAccessibilityOptionsService facilityServiceService)
        {
            _facilityServiceService = facilityServiceService;
        }

        [HttpGet("CreateAccessibilityOption")]
        public async Task<IResult> CreateyAccessibilityOption(string typename)

            => (await _facilityServiceService.CreateAccessibilityOption(typename)).GetResponse();

        [HttpGet("GetAccessibilityOptionsDropDown")]
        public async Task<IResult> GetAccessibilityOptionsDropDown()

            => (await _facilityServiceService.GetAccessibilityOptionsDropDown()).GetResponse();

    }
}
