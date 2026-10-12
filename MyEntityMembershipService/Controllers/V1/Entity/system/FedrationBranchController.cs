using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.System.EntityBranchService;

namespace MyEntityMembershipService.Controllers.V1.Entity.system
{
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class FedrationBranchController : ControllerBase
    {
        private readonly IFedrationBranchService fedrationBranchService;

        public FedrationBranchController(IFedrationBranchService fedrationBranchService)
        {
            this.fedrationBranchService = fedrationBranchService;
        }

        [HttpPost("CreateFedrationBranch")]
        public async Task<IResult> CreateFedrationBranch(string enName, string arName, int TenantId)
        {
            var result = await fedrationBranchService.CreateFedrationBranch(enName, arName, TenantId);
            return result.GetResponse();
        }
        [HttpGet("GetBranchDropDown")]
        public async Task<IResult> GetBranchDropDown( int languageType)
        {
            //int tenantId = int.Parse(User.FindFirst("tenantId")?.Value);
            var result = await fedrationBranchService.GetBranchDropDown(2, languageType);
            return result.GetResponse();
        }

       
    }
}
