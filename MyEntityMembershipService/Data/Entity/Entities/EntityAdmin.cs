using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class EntityAdmin : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserId { get; set; }
        public int? RoleId { get; set; }
        public EntityAdminStatus EntityAdminStatus { get; set; }
        public Tenant Tenant { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }

    }
}
