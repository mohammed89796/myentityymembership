using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class PaymentInvoice
    {
        //TODO: Change Id From Int To Long
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PaymentMethod { get; set; } // Cash, Visa
        public decimal Amount { get; set; } 
        public string Currency { get; set; }  // EGP or USD
        public RequestStatus Status { get; set; } // e.g., "Pending", "Completed", "Failed"
        public long? TransactionId { get; set; } // Unique from payment gateway
        public long PaymentIntentId { get; set; } 
        public List<PaymentItem> Items { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }



        public Refund Refund { get; set; }




    }



}
