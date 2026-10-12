using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos
{
    public class GetFedrationPaymentRequestToReturnDto
    {
        public int PaymentRequestId { get; set; }
        public string EntityName { get; set; }
        public string? SuperEntityName { get; set; }

        public int EntityId { get; set; }
        public int? SuperEntityId { get; set; }

        public int NumberOfRequests { get; set; }

        public int BatchId { get; set; }
        //public string BatchNumber { get; set; }

        public DateTime DateOfSubmission { get; set; }


        public PaymentStatus PaymentStatus { get; set; }

        public List<string> InvoiceUrl { get; set; } = new List<string>();

        public string? Reason { get; set; }

        public string? RejectionUrl { get; set; }

        public string? ConfirmationUrl { get; set; }

        public string? ConfirmationNotes { get; set; }

        public double AmountPaid { get; set; }

        public string? BranchAdminReason { get; set; }

        public string? BranchAdminRejectionUrl { get; set; }

        public string? BranchAdminConfirmationUrl { get; set; }

        public string? BranchAdminConfirmationNotes { get; set; }


        public OperationType OperationType { get; set; }

        public BranchAdminStatus BranchAdminStatus { get; set; }
        public EntityPaymentSummaryDto EntityPaymentSummaryDto { get; set; }

        public string RequestType { get; set; }

        //public int TotalAmountDue { get; set; }
        //public List<string> Members { get; set; } = new List<string>();

    }

}
