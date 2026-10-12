namespace MyEntityMembershipService.DTOs.Entity.TenantDtos
{
    public class ReachOutToTenant
    {
        public int TenantId { get; set; }

        public string Subject { get; set; }

        public string OperationType { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }
        public IFormFile? Attachment { get; set; }

        public string? Compliant { get; set; }

        public string? SpecificSelection { get; set; }

        public string? TransactionId { get; set; }

        public DateTime? TransactionDateTime { get; set; }

        public decimal? Amount { get; set; }

        public string? RequestType { get; set; }

        public string Message { get; set; }

    }
}
