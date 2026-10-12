using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.DTOs
{
    public class PaymentBatchDto : BaseEntity
    {

        public int EntityId { get; set; }
        public int? SuperEntityId { get; set; }
        public string? EntityName { get; set; }
        public string? SuperEntityName { get; set; }

        //  public Tenant Entity { get; set; }
        //  public Tenant? SuperEntity { get; set; }
        public int BatchId { get; set; }
        //  public Batch Batch { get; set; }

        //public List<string>? InvoiceUrl { get; set; } = new List<string>();

        //public ICollection<NewPaymentRequest> NewPaymentRequests = new HashSet<NewPaymentRequest>();

        //public PaymentType PaymentType { get; set; }
        //public PaymentStatus PaymentStatus { get; set; }

        // public AreaConfiguration AreaConfiguration { get; set; }

        public int? AreaConfigurationId { get; set; }

        public double TotalAmountDue { get; set; }

        public double AmountPaid { get; set; }

        public string? Reason { get; set; }

        public string? RejectionUrl { get; set; }

        public string? ConfirmationUrl { get; set; }

        public string? ConfirmationNotes { get; set; }

        //public BranchAdminStatus BranchAdminStatus { get; set; }

        //public OperationType OperationType { get; set; }


        public string? BranchAdminReason { get; set; }

        public string? BranchAdminRejectionUrl { get; set; }

        public string? BranchAdminConfirmationUrl { get; set; }

        public string? BranchAdminConfirmationNotes { get; set; }
    }
}
