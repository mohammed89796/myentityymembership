using MyEntityMembershipService.Data.Entity.Entities.Enums;
using System.Text.Json.Serialization;

namespace MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos
{
    public class PaymentRequestDto
    {
        public decimal Amount => Items.Sum(x => x.amount );
        public string Currency { get; set; } = "EGP";
        [JsonIgnore]
        public List<object> PaymentMethods { get; set; } = new List<object> { "card", 4875691 }; // Mixed type list (ID and string)
        public List<ItemDto> Items { get; set; }

        public CustomerDto Customer { get; set; }
        public string? RedirectionUrl { get; set; }
        public string UserId { get; set; }





    }




}
