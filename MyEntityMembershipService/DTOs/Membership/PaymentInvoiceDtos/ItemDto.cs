
using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos
{
    public class ItemDto
    {
        public string name { get; set; }
        private int Amount;

        public int amount
        {
            get { return Amount; }
            set { Amount = value *100; }
        }

        public string description { get; set; }
        
        public int PaymentTypeId { get; set; }
        public int ItemId { get; set; }
        public string? FacilityName { get; set; }
        public string? VenueName { get; set; }

    }

    public class ItemResultDto
    {
        public string name { get; set; }


        public int amount
        {
            get;
            set;
        }

        public string description { get; set; }

        public int PaymentTypeId { get; set; }
        public int ItemId { get; set; }

    }




}
