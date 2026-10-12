namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class BankAccount : BaseEntity
    {
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; }
    }
}
