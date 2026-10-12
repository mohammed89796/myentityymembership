using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.RequestAuthorizationService;

namespace MyEntityMembershipService.Controllers.V1.Entity
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class RequestAuthorizationController : ControllerBase
    {
        private readonly IRequestAuthorizationService requestAuthorizationService;

        public RequestAuthorizationController(IRequestAuthorizationService requestAuthorizationService)
        {
            this.requestAuthorizationService = requestAuthorizationService;
        }

        [HttpGet("GetAllRequestAuthorizations")]
        public async Task<IResult> GetAllRequestAuthorizations()
        {
            var result = await requestAuthorizationService.GetAllRequestAuthorizations();
            return result.GetResponse();
        }

        [HttpPost("AddRequestAuthorization")]
        public async Task<IResult> AddRequestAuthorization([FromBody] RequestAuthorizationDto requestAuthorizationDto)
        {
            var result = await requestAuthorizationService.AddRequestAuthorization(requestAuthorizationDto);
            return result.GetResponse();
        }
    }
}
