
using MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos;

namespace MyEntityMembershipService.DTOs.Membership.TransactionDtos
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string PaymentMethod { get; set; }

        public double AmountPaid { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int PaymentRequestId { get; set; }
        public PaymentRequestDto paymentRequestDto  { get; set; }

    }
}
