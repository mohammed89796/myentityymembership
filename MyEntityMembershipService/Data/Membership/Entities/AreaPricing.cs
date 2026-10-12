namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class AreaPricing : BaseEntity
    {

        // belt name
        public string BeltName { get; set; }
        public int BeltId { get; set; }
        public int MembershipPrice { get; set; }
        public int TransferPrice { get; set; }
        public int LoanPrice { get; set; }

        public int AreaId { get; set; }
        public Area Area { get; set; }

    }

}
