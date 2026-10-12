using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.TurfTypesService;

namespace MyEntityMembershipService.Controllers.V1.Entity.Venue
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class TurfTypesController : ControllerBase
    {
        private readonly ITurfTypesService _TurfTypeservice;

        public TurfTypesController(ITurfTypesService TurfTypeservice)
        {
            _TurfTypeservice = TurfTypeservice;
        }

        [HttpGet("CreatTurfType")]
        public async Task<IResult> CreateTurfType(string typename)

            => (await _TurfTypeservice.CreateTurfType(typename)).GetResponse();

        [HttpGet("GetTurfTypesDropDown")]
        public async Task<IResult> GetTurfTypesDropDown()

            => (await _TurfTypeservice.GetTurfTypesDropDown()).GetResponse();

    }
}
