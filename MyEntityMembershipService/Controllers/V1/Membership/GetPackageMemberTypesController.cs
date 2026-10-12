using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.GetPackageMemberTypesService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class GetPackageMemberTypesController : ControllerBase
    {
        private readonly IGetPackageMemberTypesService _service;

        public GetPackageMemberTypesController(IGetPackageMemberTypesService service)
        {
            _service = service;
        }

        [HttpGet("GetPackageMemberTypesIdsByPackageId/{packageId:int}")]
        public async Task<IResult> GetPackageMemberTypesIdsByPackageId(int packageId)
        {
            var result = await _service.GetPackageMemberTypeIdsAsync(packageId);
            return result.GetResponse();
            
        }

        [HttpGet("GetMemberTypeNamesByPackageId/{packageId:int}")]
        public async Task<IResult> GetMemberTypeNamesByPackageIdAsync(int packageId)
        {
            var result = await _service.GetMemberTypeNamesByIdsAsync(packageId);
            return result.GetResponse();

        }
    }
}
