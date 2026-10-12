//using Asp.Versioning;
//using MyMembershipService.Services.TenantServ;
//using MyMembershipService.Dtos.TenantDtos;
//using MyMembershipService.Extensions;
//using Microsoft.AspNetCore.Mvc;

//namespace MyMembershipService.Controllers.V1
//{

//    [Route("api/{version:apiVersion}/[controller]")]
//    [ApiVersion("1.0")]
//    [ApiController]
//    public class TenantController : ControllerBase
//    {
//        private readonly ITenantService tenantService;

//        public TenantController(ITenantService tenantService)
//        {
//            this.tenantService = tenantService;
//        }
//        [HttpPost("AddTenant")]
//        public async Task<IResult> AddTenant(AddTenantDto tenantDto)
//        {
//            var result = await tenantService.AddTenant(tenantDto);
//            return result.GetResponse();
//        }
//        [HttpGet("GetAllTenants")]
//        public async Task<IResult> GetAllTenants(string? searchName)
//        {
//            var result = await tenantService.GetAllTenants(searchName);
//            return result.GetResponse();
//        }


//        [HttpGet("GetTenantById/{tenantId}")]

//        public async Task<IResult> GetTenantById(int tenantId)

//            => (await tenantService.GetTenantByIdAsync(tenantId)).GetResponse();
//    }
//}
