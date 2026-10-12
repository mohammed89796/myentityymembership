namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos
{
    public class ConfirmPaymentDto
    {
        public int PaymentRequestId { get; set; }

        public IFormFile? ConfirmationDocument { get; set; }

        public string? ConfirmationNotes { get; set; }
    }
}
