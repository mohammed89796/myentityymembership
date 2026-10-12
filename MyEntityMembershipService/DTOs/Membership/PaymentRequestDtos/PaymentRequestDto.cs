using MyEntityMembershipService.DTOs.Membership.TransactionDtos;

namespace MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos
{
    public class PaymentRequestDto
    {
        public int Id { get; set; }
        public double MembershipFees { get; set; }
        public double ServiceCharges { get; set; }
        public double Taxes { get; set; }
        public double TotalAmount { get; set; }
        public string? Comments { get; set; }

        public int TenantId { get; set; }
        public int MembershipRequestId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        //public TenantDto TenantDto { get; set; }
        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    }
}
