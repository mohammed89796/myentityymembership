using MyEntityMembershipService.Data.Entity.Entities.Enums;
using System.ComponentModel;

namespace MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos
{
    public class PaymentInvoiceDto
    {
        [DisplayName("Invoice Id")]
        public int Id { get; set; }
        public decimal Amount { get; set; }

        [DisplayName("Date")]
        public DateTime PaymentDate  { get; set; }
  
    }

    
}
