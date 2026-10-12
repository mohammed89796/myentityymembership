using MyEntityMembershipService.DTOs.Entity.FederationDtos;
using MyEntityMembershipService.DTOs.Membership;
using MyEntityMembershipService.DTOs.Membership.BulckUploadDtos;
using MyEntityMembershipService.DTOs.Membership.Swimming;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.BulckUploadXML
{
    public interface IExcelTemplateService
    {
        /// <summary> Builds an Excel template for uploading entities, including dropdowns and validation.</summary>
        Task<byte[]> GenerateEntityTemplateExcelAsync(int federationId, string userId);
        Task<Response<List<ExcelValidationError>>> ValidateExcelTemplateAsync(IFormFile file, int federationId, string userId);
        Task<Response<string>> AddEntitiesFromExcelAsync(IFormFile file, int federationId);



        /// <summary> Builds an Excel template for uploading branch entities, including dropdowns and validation.</summary>
        Task<byte[]> GenerateBranchEntityTemplateExcelAsync();
        Task<Response<List<ExcelValidationError>>> ValidateBranchEntityTemplateAsync(IFormFile file, int federationId);
        Task<Response<string>> UploadBranchEntitiesFromExcelAsync(IFormFile file, int federationId, int fedrationBranchId);



        /// <summary> Builds an Excel template for uploading new members, including dropdowns and validation.</summary>
        Task<byte[]> GenerateNewMembersTemplateExcelAsync();
        Task<Response<List<ExcelValidationError>>> ValidateNewMembersTemplateAsync(IFormFile file);


        Task<Response<List<ExcelValidationError>>> ValidateMembershipTemplateAsync(IFormFile file);
        Task<Response<string>> UploadMembershipsFromExcelAsync(IFormFile file);

        Task<byte[]> DownloadMembersTemplateExcelAsync(int? tenantId);
        Task<byte[]> DownloadSwimmingMembersExcelAsync();
        Task<byte[]> ExportMembersSportLevelsExcelAsync();

    }
}
