using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos
{
    public class GetAllEntityPaymentRequestsToReturnDto
    {

        public int PaymentRequestId { get; set; }
        public string EntityName { get; set; }
        public string? SuperEntityName { get; set; }

        public int EntityId { get; set; }
        public int? SuperEntityId { get; set; }

        public int NumberOfRequests { get; set; }


        public DateTime DateOfSubmission { get; set; }

        public string? Reason { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public double TotalAmountDue { get; set; }

        public double AmountPaid { get; set; }

        public string RequestType { get; set; }

        public BranchAdminStatus BranchAdminStatus { get; set; }
    }
}
