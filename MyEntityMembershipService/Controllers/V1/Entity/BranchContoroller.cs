using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.BranchService;

namespace MyEntityMembershipService.Controllers.V1.Entity
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class BranchController : ControllerBase
    {
        private readonly IBranchService branchService;

        public BranchController(IBranchService branchService)
        {
            this.branchService = branchService;
        }

        [HttpGet("GetAllBranches")]
        public async Task<IResult> GetAllBranches()
        {
            var result = await branchService.GetAllBranchesAsync();
            return result.GetResponse();
        }

        [HttpGet("GetAllBranchesByTenant")]
        public async Task<IResult> GetAllBranchesByTenant()
        {
            int.TryParse(User.GetTenantId(), out var tenantId);
            var userId = User.GetUserId();
            var result = await branchService.GetAllBranchesByTenantIdAsync(tenantId, userId);
            return result.GetResponse();
        }
        [HttpGet("Tenant/{tenantId}")]
        public async Task<IResult> GetAllBranchesByTenantId(int tenantId)
        {
            var userId = User.GetUserId();
            var result = await branchService.GetAllBranchesByTenantIdAsync(tenantId, userId);
            return result.GetResponse();
        }

        [HttpGet("GetBranchById/{branchId}")]
        public async Task<IResult> GetBranchById(int branchId)
        {
            var result = await branchService.GetBranchByIdAsync(branchId);
            return result.GetResponse();
        }
        [HttpGet("GetAllBranchesByTenantId/{tenantId}")]
        public async Task<IResult> GetAllBranchesByTenantId(int tenantId, string? searchTerm, int? pageIndex, int? pageSize)
        {
            var tokenTenantId = int.Parse(User.GetTenantId());
            if (tokenTenantId > 0)
                tenantId = tokenTenantId;
            var result = await branchService.GetBranchByTenantIdAsync(tenantId, searchTerm, pageIndex, pageSize);
            return result.GetResponse();
        }
        [HttpGet("GetBranchDetailsByIdAsync/{branchID}")]
        public async Task<IResult> GetBranchDetailsByIdAsync(int branchID)
        {
            var result = await branchService.GetBranchDetailsByIdAsync(branchID);
            return result.GetResponse();
        }
        [HttpGet("GetBranchCardList/{pageNumber}/{tenantID}")]
        public async Task<IResult> GetBranchCardList(int pageNumber)
        {
            int.TryParse(User.GetTenantId(), out var tenantId);
            var result = await branchService.GetBranchCardList(pageNumber, tenantId);
            return result.GetResponse();
        }

        [HttpPost("AddBranch")]
        public async Task<IResult> AddBranch([FromForm] AddBranchDto branchDto)
        {
            var tenantId = int.Parse(User.GetTenantId());
            if (tenantId > 0)
                branchDto.TenantId = tenantId;
            var result = await branchService.AddBranchAsync(branchDto);
            return result.GetResponse();
        }

        [HttpPost("AddBranchOperatingHours")]
        public async Task<IResult> AddBranchOperatingHours([FromBody] BranchOperatingHoursDto? operatingHoursDto)
        {

            var result = await branchService.AddBranchOperatingHoursAsync(operatingHoursDto);
            return result.GetResponse();
        }


        [HttpPut("UpdateBranch")]
        public async Task<IResult> UpdateBranch([FromBody] BranchDto branchDto)
        {
            var tenantId = int.Parse(User.GetTenantId());
            if (tenantId > 0)
                branchDto.TenantId = tenantId;
            var result = await branchService.UpdateBranchAsync(branchDto);
            return result.GetResponse();
        }
        [HttpPut("EditBranch")]
        public async Task<IResult> EditBranchAsync([FromForm] EditBranchDto branchDto)
        {

            var result = await branchService.EditBranchAsync(branchDto);
            return result.GetResponse();
        }
        [HttpDelete("RemoveBranchAsync")]
        public async Task<IResult> RemoveBranchAsync(int branchID)
        {
            var result = await branchService.RemoveBranchAsync(branchID);
            return result.GetResponse();
        }
        [HttpDelete("RemoveBranchMediaAsync")]
        public async Task<IResult> RemoveBranchMediaAsync(int branchId, int mediaId)
        {
            var result = await branchService.RemoveBranchMediaAsync(branchId, mediaId);
            return result.GetResponse();
        }
        [HttpGet("GetBranchEditDetailsByIdAsync/{branchId}")]
        public async Task<IResult> GetBranchEditDetailsByIdAsync(int branchId)
        {
            var result = await branchService.GetBranchEditDetailsByIdAsync(branchId);
            return result.GetResponse();
        }

        [HttpGet("GetBranchCardsByTenantID/{pageNumber}/{tenantID}")] //Mobile only
        public async Task<IResult> GetBranchCardsByTenantID(int pageNumber, int tenantID)
        {
            var result = await branchService.GetBranchCardsByTenantID(pageNumber, tenantID);
            return result.GetResponse();
        }
        [HttpGet("GetMobileBranchDetailsByID/{branchId}")] //Mobile only
        public async Task<IResult> GetBranchMobileDetailsByID(int branchId)
        {
            var result = await branchService.GetBranchMobileDetailsByID(branchId);
            return result.GetResponse();
        }
        [HttpGet("GetMobileBranchFacilities/{branchId}")] //Mobile only
        public async Task<IResult> GetMobileBranchFacilities(int branchId)
        {
            var result = await branchService.GetMobileBranchFacilities(branchId);
            return result.GetResponse();
        }

        [HttpPost("AddBranchNewMedia")]
        public async Task<IResult> AddBranchNewMedia([FromForm] List<IFormFile> newMedia, int branchID)
        {
            var result = await branchService.AddBranchNewMedia(newMedia, branchID);
            return result.GetResponse();
        }
        [HttpPut("UpdateBranchOperatingHoursAsync")]
        public async Task<IResult> UpdateBranchOperatingHoursAsync(EditBranchOperatingHoursDto branchDto)
        {
            var result = await branchService.UpdateBranchOperatingHoursAsync(branchDto);
            return result.GetResponse();
        }


        [HttpGet("Branches")]
        public async Task<IResult> GetBranchesByTenant([FromQuery]BranchLookupSearchDto dto)
        {
            var tenantId = User.GetParsedTenantId();
            dto.TenantId = tenantId;
            var result = await branchService.GetBranchesByTenantAsync(dto);
            return result.GetResponse();
        }
        
    }
}
