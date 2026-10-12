namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class Transaction:BaseEntity
    {
        public string PaymentMethod { get; set; }

        public double AmountPaid { get; set; }
        //navigationProperities
        public int PaymentRequestId { get; set; }
        public virtual PaymentRequest? PaymentRequest { get; set; }
    }
}
