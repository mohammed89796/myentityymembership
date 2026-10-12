using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.System.FedrationEntityTypesService;

namespace MyEntityMembershipService.Controllers.V1.Entity.system
{
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class FedrationEntityTypesController : ControllerBase
    {
        private readonly IFedrationEntityTypesService fedrationEntityTypesService;

        public FedrationEntityTypesController(IFedrationEntityTypesService fedrationEntityTypesService)
        {
            this.fedrationEntityTypesService = fedrationEntityTypesService;
        }

        [HttpPost("CreateFedrationEntityType")]
        public async Task<IResult> CreateFedrationEntityType(string name)
        {
            var result = await fedrationEntityTypesService.CreateFedrationEntityType(name);
            return result.GetResponse();
        }

        [HttpGet("GetFedEntityTypes")]
        public async Task<IResult> GetFedEntityTypes()
        {
            var result = await fedrationEntityTypesService.GetFedEntityTypes();
            return result.GetResponse();
        }

       
    }
}
