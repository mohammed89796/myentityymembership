using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos
{
    public class GetFedrationPaymentRequestDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? SearchTerm { get; set; }
        public OperationType? OperationType { get; set; }
        public DateOnly? DateOfSubmission { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
