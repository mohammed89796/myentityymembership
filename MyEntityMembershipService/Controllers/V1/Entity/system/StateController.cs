using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.System.StateService;

namespace MyEntityMembershipService.Controllers.V1.Entity.system
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class StateController : ControllerBase
    {
        private readonly IStateService stateService;

        public StateController(IStateService stateService)
        {
            this.stateService = stateService;
        }

        [HttpGet("CreateState")]
        public async Task<IResult> CreateState(int countryID, string stateName)
        {
            var result = await stateService.CreateState(countryID, stateName);
            return result.GetResponse();
        }
        [HttpGet("GetAllStates")]
        public async Task<IResult> GetAllStates(int countryID)
        {
            var result = await stateService.GetAllStates(countryID);
            return result.GetResponse();
        }

       
    }
}
