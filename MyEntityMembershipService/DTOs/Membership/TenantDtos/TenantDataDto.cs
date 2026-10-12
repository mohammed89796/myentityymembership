namespace MyEntityMembershipService.DTOs.Membership.TenantDtos
{
    public class TenantDataDto
    {
        public int EntityId { get; set; }
        public int SuperEntityId { get; set; }
        public string EntityName { get; set; }
        public string SuperEntityName { get; set; }
    }
}
