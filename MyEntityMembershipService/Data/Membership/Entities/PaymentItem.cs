using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{

    public class PaymentItem
    {
        public int Id { get; set; }
        public string name { get; set; }
        public decimal amount { get; set; }
        public string description { get; set; }

        public PaymentInvoice PaymentInvoice { get; set; }
        public int PaymentInvoiceId { get; set; }

        public PaymentInvoiceType PaymentType { get; set; }
        public int PaymentTypeId { get; set; }

        public int ItemId { get; set; }
        public string? FacilityName { get; set; }
        public string? VenueName { get; set; }

    }




}
