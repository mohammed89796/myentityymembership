namespace Common.RabbitMq.Tenant
{
    public class TenantCreatedDto
    {
        public int EntityId { get; set; }
        
        public string Name { get; set; }

        public string ImageUrl { get; set; }
    }
}
