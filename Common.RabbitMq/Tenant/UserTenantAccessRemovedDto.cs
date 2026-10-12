namespace Common.RabbitMq.Tenant
{
    public class UserTenantAccessRemovedDto
    {
        public string UserId { get; set; }
        public int TenantId { get; set; }
    }
}
