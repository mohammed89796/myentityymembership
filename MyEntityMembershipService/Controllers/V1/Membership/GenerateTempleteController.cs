using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.Swimming;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.BulckUploadXML;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]

    [ApiVersion("1.0")]
    [Authorize]

    public class GenerateTempleteController : ControllerBase
    {
        [HttpGet("Download-Entity-Template-Excel")]
        public async Task<IActionResult> GenerateEntityTemplateExcelAsync([FromServices] IExcelTemplateService service)
        {
            var federationId = int.Parse(User.FindFirst("tenantId")?.Value);
            string userId = User.GetUserId()!;

            var fileBytes = await service.GenerateEntityTemplateExcelAsync(federationId,userId);
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UploadEntityTemplate.xlsx"
                );
        }

        [HttpPost("validate-entity-template")]
        public async Task<IResult> ValidateEntityTemplate(IFormFile file, [FromServices] IExcelTemplateService service)
        {
            var federationId = int.Parse(User.FindFirst("tenantId")?.Value!);
            var userId = User.GetUserId()!;

            var result = await service.ValidateExcelTemplateAsync(file, federationId, userId);
            return result.GetResponse();
        }

        [HttpPost("upload-entity-template")]
        public async Task<IResult> UploadEntityTemplate(IFormFile file, [FromServices] IExcelTemplateService service)
        {
            var federationId = int.Parse(User.FindFirst("tenantId")?.Value!);

            var result = await service.AddEntitiesFromExcelAsync(file, federationId);

            return result.GetResponse();
        }




        [HttpGet("Download-Branch-Entity-Template-Excel")]
        public async Task<IActionResult> GenerateBranchEntityTemplateExcelAsync([FromServices] IExcelTemplateService service)
        {        
            var fileBytes = await service.GenerateBranchEntityTemplateExcelAsync();
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "BranchEntityTemplate.xlsx");
        }
     

        [HttpPost("validate-branch-entity-template")]
        public async Task<IResult> ValidateBranchEntityTemplate(IFormFile file, [FromServices] IExcelTemplateService service)
        {
            int federationId = int.Parse(User.FindFirst("tenantId")!.Value);
            var result = await service.ValidateBranchEntityTemplateAsync(file, federationId);
            return result.GetResponse();
        }

        [HttpPost("upload-branch-entities")]
        public async Task<IResult> UploadBranchEntities(IFormFile file, int fedrationBranchId, [FromServices] IExcelTemplateService service)
        {
            int federationId = int.Parse(User.FindFirst("tenantId")!.Value);
            var result = await service.UploadBranchEntitiesFromExcelAsync(file, federationId, fedrationBranchId);
            return result.GetResponse();
        }





        [HttpGet("Download-New-Memebers-Template-Excel")]
        public async Task<IActionResult> GenerateNewMembersTemplateExcel([FromServices] IExcelTemplateService service)
        {
            var fileBytes = await service.GenerateNewMembersTemplateExcelAsync();
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "NewMembersTemplate.xlsx");
        }

        [HttpGet("Download-Members-Template-Excel")]
        public async Task<IActionResult> DownloadMembersTemplateExcel([FromServices] IExcelTemplateService service)
        {
            var tenantId = User.GetParsedTenantId();
            var fileBytes = await service.DownloadMembersTemplateExcelAsync(tenantId);
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "MembersTemplate.xlsx");
        }

        [HttpGet("download-swimming-members")]
        public async Task<IActionResult> DownloadMembershipTemplateExcel([FromServices] IExcelTemplateService service)
        {
            var fileBytes = await service.DownloadSwimmingMembersExcelAsync();
            return File(
                   fileBytes,
                   "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                   $"SwimmingMembers_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
               );
        }


        [HttpGet("ExportMembersSportLevels")]
        public async Task<IActionResult> ExportMembersSportLevels([FromServices] IExcelTemplateService service)
        {
            var fileBytes = await service.ExportMembersSportLevelsExcelAsync();

            if (fileBytes == null || fileBytes.Length == 0)
                return BadRequest("No data available to export");

            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"MembersSportLevels_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpPost("validate-New-Members-template")]
        public async Task<IResult> ValidateNewMembersTemplate(IFormFile file, [FromServices] IExcelTemplateService service)
        {
            var result = await service.ValidateNewMembersTemplateAsync(file);
            return result.GetResponse();
        }
        [HttpPost("validate-Members-template")]
        public async Task<IResult> ValidateMembershipTemplateAsync(IFormFile file, [FromServices] IExcelTemplateService service)
        {
            var result = await service.ValidateMembershipTemplateAsync(file);
            return result.GetResponse();
        }
        [HttpPost("upload-Memberships")]
        public async Task<IResult> UploadMembershipsFromExcelAsync(IFormFile file, [FromServices] IExcelTemplateService service)
        {
            var result = await service.UploadMembershipsFromExcelAsync(file);
            return result.GetResponse();
        }

    }
}
