using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class PaymentRequest:BaseEntity
    {
        public  double MembershipFees { get; set; }
        public double ServiceCharges { get; set; }
        public double Taxes { get; set; }
        public double TotalAmount { get; set; }
        public string? Comments { get; set; }
        public RequestStatus RequestStatus { get; set; }

        public int TenantId { get; set; }
        public int MembershipRequestId { get; set; }
        //navigation Properities
        public virtual MembershipRequest? MembershipRequest { get; set; }    
        public virtual ICollection<Transaction>?Transactions { get; set; }

    }
}
