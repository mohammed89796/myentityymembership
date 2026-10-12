using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class Refund
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public decimal RefundAmount { get; set; }
        public RequestStatus Status { get; set; } // e.g., "Pending", "Completed", "Failed"
        public DateTime RefundDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        public PaymentInvoice Payment { get; set; }
    }



}
