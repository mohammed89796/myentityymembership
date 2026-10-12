using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.AtheleteDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.AtheleteService;

namespace MyEntityMembershipService.Controllers.V1.Entity
{

    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class AtheletesController : ControllerBase
    {
        private readonly IAtheleteService _atheleteService;

        public AtheletesController(IAtheleteService atheleteService)
        {
            _atheleteService = atheleteService;
        }

        [HttpPost("athelete")]
        public async Task<IResult> CreateAtheleteAsync([FromForm] AddAtheleteDto dto)
            => (await _atheleteService.CreateAtheleteAsync(dto)).GetResponse();

        [HttpGet("athelete/{tenantId}")]
        public async Task<IResult> GetAllDraftAtheletesAsync(int tenantId)
            => (await _atheleteService.GetDraftAtheleteListAsync(tenantId)).GetResponse();


    }
}
