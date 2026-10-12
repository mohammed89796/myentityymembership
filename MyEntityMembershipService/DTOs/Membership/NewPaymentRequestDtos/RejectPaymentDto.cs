namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos
{
    public class RejectPaymentDto
    {
        public int PaymentRequestId { get; set; }

        public string Reason { get; set; }

        public IFormFile? RejectionDocument { get; set; }
    }
}
