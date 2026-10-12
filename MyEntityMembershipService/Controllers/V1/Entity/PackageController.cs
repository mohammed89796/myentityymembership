using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;
using MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.PackageService;

namespace MyEntityMembershipService.Controllers.V1.Entity
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class PackageController : ControllerBase
    {
        private readonly IPackageService packageService;
        private readonly MyEntityMembershipService.Services.Membership.PackageServ.IPackageService packageServ;
        public PackageController(IPackageService packageService, Services.Membership.PackageServ.IPackageService packageServ)
        {
            this.packageService = packageService;
            this.packageServ = packageServ;
        }

        [HttpPost("AddPackage")]
        public async Task<IResult> AddPackage(
        [ModelBinder(BinderType = typeof(AddPackageModelBinder))] dynamic model)
        {
            var packageDto = model.PackageDto as AddPackageDto;
            var file = model.File as IFormFile;


            // Process the file
            if (file != null && file.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    var fileContent = stream.ToArray();
                    // Process the file content (e.g., save to disk, upload to cloud, etc.)
                }
            }

            // Process the packageDto
            var result = await packageService.AddPackage(packageDto, file);
            return result.GetResponse();
        }




        [HttpGet("GetAllPackages")]
        public async Task<IResult> GetAllPackageByTenantId([FromQuery]int? tenatId)
        {
            var result = await packageService.GetAllPackageByTenantId(tenatId);
            return result.GetResponse();
        }
        [HttpGet("GetSubscribedPackagesByTenantId")]
        public async Task<IResult> GetSubscribedPackagesByTenantId(int tenatId)
        {
            var result = await packageService.GetSubscribedPackagesByTenantId(tenatId);
            return result.GetResponse();
        }
        [HttpGet("GetPackageById/{packageId}")]
        public async Task<IResult> GetPackageById(int packageId)
        {
            var result = await packageService.GetPackageById(packageId);
            return result.GetResponse();
        }
        [HttpGet("GetPackageByIdAsync/{packageId}")]
        public async Task<IResult> GetPackageIdAsync(int packageId)
        {
            var result = await packageService.GetPackageByIdAsync(packageId);
            return result.GetResponse();
        }
        [HttpGet("GetPackagesByTenantId/{tenantId}")]
        public async Task<IResult> GetPackagesByTenantId(int tenantId)
        {
            var result = await packageService.GetPackagesByTenantIdAsync(tenantId);
            return result.GetResponse();
        }

        [HttpGet("GetAllPackagesByTenantId/{tenantId}")]
        public async Task<IResult> GetAllPackagesByTenantId()
        {
            int.TryParse(User.GetTenantId(), out var tenantId);

            var result = await packageService.GetAllPackagesByTenantId(tenantId);
            return result.GetResponse();
        }

        [HttpPost("GetPackagesByTenantIdAndListOfIds/{tenantId}")]
        public async Task<IResult> GetPackagesByTenantId([FromBody] List<int> packagesIds)
        {
            int.TryParse(User.GetTenantId(), out var tenantId);
            var result = await packageService.GetAllPackagesByTenantIdAndListOfIds(tenantId, packagesIds);
            return result.GetResponse();
        }

        [HttpDelete("SoftDeletePackage/{packageId}")]
        public async Task<IResult> SoftDeletePackage(int packageId)
        {
            var result = await packageService.SoftDeletePackage(packageId);
            return result.GetResponse();
        }

        [HttpGet("GetSuperPackages")]
        public async Task<IResult> GetSuperPackages()
        {
            var result = await packageService.GetSuperPackagesAsync();
            return Results.Ok(result);
        }

        [HttpGet("SupPackage/{superPackageId}")]
        public async Task<IResult> GetSupPackages(int superPackageId)
        {
            var result = await packageService.GetSupPackagesAsync(superPackageId);
            return Results.Ok(result);
        }

        [HttpGet("GetPackageBranchesAsync/{packageId}")]
        public async Task<IResult> GetPackageBranchesAsync(int packageId, [FromQuery] PackageBranchListDto packageBranchListDto)
        {
            var result = await packageService.GetPackageBranchesAsync(packageId, packageBranchListDto);
            return Results.Ok(result);
        }
        [HttpGet("GetAccessedFacilitiesWithVenuesAsync/{packageId}")]
        public async Task<IResult> GetAccessedFacilitiesWithVenuesAsync(int packageId, [FromQuery] List<int> branchIds)
        {
            var result = await packageService.GetAccessedFacilitiesWithVenuesAsync(packageId, branchIds);
            return Results.Ok(result);
        }

        [HttpPost("AddPackageWithSubPackages")]
        public async Task<IResult> AddPackageWithSubPackages([FromForm] AddPackageWithSubPackagesDto dto)
        {
            var result = await packageServ.AddPackage(dto);

            return result.GetResponse();
        }

        [HttpGet()]
        public async Task<IResult> GetAllPackagesAsync([FromQuery] GetAllPackageFilterResponseDto filterDto)
        {
            var tenantId = User.GetParsedTenantId();
            filterDto.TenantId = tenantId;

            var result = await packageServ.GetAllPackagesAsync(filterDto);
            return result.GetResponse();
        }

        [HttpGet("{packageId}")]
        public async Task<IResult> GetPackageByIdAsync(int packageId)
        {
            var tenantId = User.GetParsedTenantId();
            var result = await packageServ.GetPackageByIdAsync(packageId, tenantId);
            return result.GetResponse();
        }

        

        [HttpPut("Activation")]
        public async Task<IResult> UpdatePackageActivationStatusAsync(UpdatePackageActivitionDto dto)
        {
            var tenantId = User.GetParsedTenantId();
            dto.TenantId = tenantId;
            var result = await packageServ.UpdatePackageActivationStatusAsync(dto);
            return result.GetResponse();
        }

        [HttpGet("{packageId}/members")]
        public async Task<IResult> GetAllPackageMembersAsync(
        int packageId,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageNumber = 1)
        {
            var tenantId = User.GetParsedTenantId();
            var result = await packageServ.GetAllPackageMemberAsync(tenantId, packageId, pageSize, pageNumber);
            return result.GetResponse();
        }

        [HttpGet("{packageId}/requests")]
        public async Task<IResult> GetPackageActiveRequestsAsync(
        int packageId,
        [FromQuery] ActiveRequestFilter filter,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageNumber = 1)
        {
            var tenantId = User.GetParsedTenantId();
            filter.TenantId = tenantId;

            var result = await packageServ.GetPackageActiveRequestsAsync(packageId, filter, pageSize, pageNumber);
            return result.GetResponse();
        }
        [HttpPost("GetAvailableMembersAsync")]
        public async Task<IResult> GetAvailableMembersAsync([FromBody] GetAvailableMembersRequest request)
        {
            var result = await packageServ.GetAvailableMembersAsync(request);
            return result.GetResponse();
        }



        [HttpGet("{packageId}/subpackages")]
        public async Task<IResult> GetAllSubPackagesAsync(int packageId)
        {
            var tenantId = User.GetParsedTenantId();
            var result = await packageServ.GetAllSubPackagesAsync(tenantId, packageId);
            return result.GetResponse();
        }


        [HttpGet("{packageId}/benefits")]
        public async Task<IResult> GetPackageBenefitsAsync(int packageId)
        {
            var tenantId = User.GetParsedTenantId();
            var result = await packageServ.GetPackageBenefitsAsync(tenantId, packageId);
            return result.GetResponse();
        }

    }
}
