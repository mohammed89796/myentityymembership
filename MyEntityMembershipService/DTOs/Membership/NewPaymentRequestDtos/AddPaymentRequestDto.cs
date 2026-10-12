using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos
{
    public class AddPaymentRequestDto
    {
        public int BatchId { get; set; }

        public int EntityId { get; set; }
        public List<int> MemberRequestsIds { get; set; } = new List<int>();

        public PaymentType PaymentType { get; set; }
        public List<IFormFile?> Invoice { get; set; } = new List<IFormFile?>();

        public double AmountPaid { get; set; }

        public OperationType OperationType { get; set; }
    }
}
