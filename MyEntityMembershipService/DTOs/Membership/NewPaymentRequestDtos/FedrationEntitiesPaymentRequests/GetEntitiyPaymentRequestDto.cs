using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos.FedrationEntitiesPaymentRequests
{
    public class GetEntitiyPaymentRequestDto
    {
        public int EntityId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public string? SearchTerm { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
