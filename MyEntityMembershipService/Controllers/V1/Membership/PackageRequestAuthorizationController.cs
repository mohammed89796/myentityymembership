using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.Extensions;

using MyEntityMembershipService.Services.Membership.PackageRequestAuthorizationService;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class PackageRequestAuthorizationController : ControllerBase
    {
        private readonly IPackageRequestAuthorizationService packageRequestAuthorizationService;

        public PackageRequestAuthorizationController(IPackageRequestAuthorizationService packageRequestAuthorizationService)
        {
            this.packageRequestAuthorizationService = packageRequestAuthorizationService;
        }

        [HttpPost("AddPackageRequestAuthorization")]
        public async Task<IResult> AddPackageRequestAuthorization(PackageRequestAuthorizationDto packageRequestAuthorizationDto)
        {
            var result = await packageRequestAuthorizationService.AddPackageRequestAuthorization(packageRequestAuthorizationDto);
            return result.GetResponse();
        }
        [HttpPost("AddPackageRequestAuthorizations")]
        public async Task<IResult> AddPackageRequestAuthorizations(List<PackageRequestAuthorizationDto> packageRequestAuthorizationDtos)
        {
            var result = await packageRequestAuthorizationService.AddPackageRequestAuthorizations(packageRequestAuthorizationDtos);
            return result.GetResponse();
        }


        [HttpGet("GetAllPackageRequestAuthorizations")]
        public async Task<IResult> GetAllPackageRequestAuthorizations()
        {
            var result = await packageRequestAuthorizationService.GetAllPackageRequestAuthorizations();
            return result.GetResponse();
        }

        [HttpGet("GetPackageRequestAuthorizationById")]
        public async Task<IResult> GetPackageRequestAuthorizationById(int id)
        {
            var result = await packageRequestAuthorizationService.GetPackageRequestAuthorizationById(id);
            return result.GetResponse();
        }

        [HttpPut("UpdatePackageRequestAuthorization")]
        public async Task<IResult> UpdatePackageRequestAuthorization(PackageRequestAuthorizationDto packageRequestAuthorizationDto)
        {
            var result = await packageRequestAuthorizationService.UpdatePackageRequestAuthorization(packageRequestAuthorizationDto);
            return result.GetResponse();
        }

        [HttpDelete("DeletePackageRequestAuthorization")]
        public async Task<IResult> DeletePackageRequestAuthorization(int id)
        {
            var result = await packageRequestAuthorizationService.DeletePackageRequestAuthorization(id);
            return result.GetResponse();
        }

        [HttpGet("GetPackageRequestAuthorizations")]
        public async Task<IResult> GetPackageRequestAuthorizationsAsync(int packageId)
        {
            var result = await packageRequestAuthorizationService.GetPackageRequestAuthorizationsAsync(packageId);
            return result.GetResponse();
        }
    }
}
