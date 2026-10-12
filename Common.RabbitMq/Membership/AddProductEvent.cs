using Common.RabbitMq.Membership.Enums;

namespace Common.RabbitMq.Membership
{
    public class ProductCreatedEvent
    {
        public int SourceId { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TenantId { get; set; }
        public ProductType ProductType { get; set; }
    }
}
