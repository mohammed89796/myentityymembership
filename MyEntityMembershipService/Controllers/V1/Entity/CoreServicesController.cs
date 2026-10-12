using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.CoreServices;

namespace MyEntityMembershipService.Controllers.V1.Entity
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class CoreServicesController : ControllerBase
    {
        private readonly ICoreServicesService _coreServicesService;

        public CoreServicesController(ICoreServicesService coreServiceService)
        {
            _coreServicesService = coreServiceService;
        }

        [HttpGet("CreateCoreService")]
        public async Task<IResult> CreateCoreService(string serviceName)

            => (await _coreServicesService.CreateCoreService(serviceName)).GetResponse();
        [HttpGet("GetCoreServicesDropDown")]
        public async Task<IResult> GetCoreServicesDropDown()

            => (await _coreServicesService.GetCoreServicesDropDown()).GetResponse();

    }
}
