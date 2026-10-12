using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos
{
    public class GetAllFedrationPaymentRequestToReturnDto
    {
        public int PaymentRequestId { get; set; }
        public string EntityName { get; set; }
        public string? SuperEntityName { get; set; }

        public string? Branch { get; set; }
        public int EntityId { get; set; }
        public int? SuperEntityId { get; set; }

        public int NumberOfRequests { get; set; }

        public int BatchId { get; set; }
        public string BatchNumber { get; set; }



        public double TotalAmountDue { get; set; }

        public double AmountPaid { get; set; }

        public DateTime DateOfSubmission { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string RequestType { get; set; }

        public BranchAdminStatus BranchAdminStatus { get; set; }

    }
}
