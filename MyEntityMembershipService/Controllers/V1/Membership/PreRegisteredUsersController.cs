using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.PreRegisteredUserService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class PreRegisteredUsersController : ControllerBase
    {
        private readonly IPreRegisteredUserService _preRegisteredUserService;

        public PreRegisteredUsersController(IPreRegisteredUserService preRegisteredUserService)
        {
            _preRegisteredUserService = preRegisteredUserService;
        }

        [HttpGet("pre-registered-users")]

        public async Task<IResult> GetPreRegisteredUserByFileNumberAsync([FromQuery] string fileNumber)


          => (await _preRegisteredUserService.GetPreRegisteredUserByFileNumberAsync(fileNumber)).GetResponse();


    }
}
