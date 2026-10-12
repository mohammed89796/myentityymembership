using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class TenantAdmin : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserId { get; set; }
        public EntityAdminStatus EntityAdminStatus { get; set; }
        public Tenant Tenant { get; set; }
        public int TenantId { get; set; }
        public ICollection<TenantAdminArea  > TenantAreaAdmins { get; set; } = new HashSet<TenantAdminArea>();
        public ICollection<TenantAdminRole> TenantAdminRoles { get; set; } = new HashSet<TenantAdminRole>();

        public ICollection<TenantAdminSport> TenantAdminSports { get; set; } = new HashSet<TenantAdminSport>();
        public ICollection<TenantAdminFacility> TenantAdminFacilities { get; set; } = new HashSet<TenantAdminFacility>();
        public ICollection<TenantAdminBranch> TenantAdminBranches { get; set; } = new HashSet<TenantAdminBranch>();
        public ICollection<TenantAdminVenue> TenantAdminVenues { get; set; } = new HashSet<TenantAdminVenue>();





    }
}
