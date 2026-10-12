namespace MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos
{
	public class FilterPaymentRequestDto
	{
		public string? Name { get; set; }

		public int? PaymentRequestId { get; set; }

		public string? Email { get; set; }

		public DateTime? PaymentRequestDate { get; set; }

		public string? RequestType { get; set; }

	}
}
