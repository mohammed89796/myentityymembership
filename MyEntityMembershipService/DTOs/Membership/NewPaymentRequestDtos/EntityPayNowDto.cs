using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos
{
    public class EntityPayNowDto
    {
        public int PaymentRequestId { get; set; }

        public PaymentType PaymentType { get; set; }

        public List<IFormFile>? Invoice { get; set; } = new List<IFormFile>();

        public double AmountPaid { get; set; }
    }
}
