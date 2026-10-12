using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos
{
    public class PaymentInvoiceDetailsDto
    {
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
        public int? TransactionId { get; set; } // Unique from payment gateway
        public int PaymentIntentId { get; set; }
        public List<ItemResultDto> Items { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


    }
}
