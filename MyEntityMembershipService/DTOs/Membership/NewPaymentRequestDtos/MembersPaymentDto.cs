namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos
{
    public class MembersPaymentDto
    {
        public int? PaymentRequestId { get; set; }

        public int MembershipRequestId { get; set; }

        public string MemberName { get; set; }

        public string BeltName { get; set; }

        public double BeltPrice { get; set; }
    }
}
