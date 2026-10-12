using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.BatchDtos
{
    public class BatchPaymentRequestDto
    {
        public int BatchId { get; set; }
        public string BatchNumber { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        public string ApplicantName { get; set; }
        public int? PackageId { get; set; }
        public string PackageName { get; set; }
        public int TotalRequests { get; set; }
        public DateTime DateOfSubmission { get; set; }
        public SubmationPlatform SubmationPlatform { get; set; }
        public BatchStatus BatchStatus { get; set; }
    }


    public class BatchPaymentRequestFilterDto
    {
        public int? TenantId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? RequestTypeId { get; set; }
        public string? SearchTerm { get; set; }
        public BatchStatus? BatchStatus { get; set; }
    }
}
