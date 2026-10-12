namespace MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos
{
    public class PaymobSettings
    {
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
        public string CheckoutUrl { get; set; }
        public int IFrameId { get; set; }
    }
}
