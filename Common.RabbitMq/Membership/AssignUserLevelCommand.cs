namespace Common.RabbitMq.Membership
{
    // Membership → Sport: "please assign this option"
    public class AssignUserLevelCommand
    {
        public string UserId { get; set; }
        public int MembershipId { get; set; }
        public int TenantId { get; set; }
        public string ChangedBy { get; set; }
        public int OptionId { get; set; }   // real int, no parsing needed
    }
}