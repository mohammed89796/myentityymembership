namespace Common.RabbitMq.Users
{
    public class InjuryStatusUpdatedEvent
    {
        public string UserId { get; set; }

        public bool ActiveInjury { get; set; }
        public int TenantId { get; set; }
        public string ChangedBy { get; set; }
    }
}
