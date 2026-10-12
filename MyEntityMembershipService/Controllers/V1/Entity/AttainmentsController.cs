using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.AttainmentDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.AttainmentService;

namespace MyEntityMembershipService.Controllers.V1.Entity
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class AttainmentsController : ControllerBase
    {
        private readonly IAttainmentService attainmentService;

        public AttainmentsController(IAttainmentService attainmentService)
        {
            this.attainmentService = attainmentService;
        }


        [HttpPost("")]
        public async Task<IResult> CreateAttainment([FromForm] AddAttainmentDto dto)
        {
            var tenantId = User.GetTenantId();
            return (await attainmentService.CreateAttainmentAsync(dto, tenantId)).GetResponse();
        }

        [HttpPut("")]
        public async Task<IResult> UpdateAttainment([FromForm] UpdateAttainmentDto dto)
            => (await attainmentService.UpdateAttainmentAsync(dto)).GetResponse();

        [HttpDelete("{attainmentId}")]
        public async Task<IResult> DeleteAttainment(int attainmentId)
            => (await attainmentService.DeleteAttainment(attainmentId)).GetResponse();
        
        [HttpGet("categories")]
        public async Task<IResult> GetAllAttainmentCategories()
            => (await attainmentService.GetAllAttainmentCategoriesAsync()).GetResponse();
        
        [HttpGet("")]
        public async Task<IResult> GetAllAttainments([FromQuery] AttainmentDto dto)
        {
            var tenantId = User.GetTenantId();
            return (await attainmentService.GetAllAttainmentsAsync(dto, tenantId)).GetResponse();
        }
 
        [HttpGet("{attainmentId}")]
        public async Task<IResult> GetAttainmentById(int attainmentId)
            => (await attainmentService.GetAttainmentByIdAsync(attainmentId)).GetResponse();

    }
}
