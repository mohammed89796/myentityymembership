using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.EntityAdminDtos
{
    public class GetEntityAdminDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public List<string> RoleIds { get; set; } = [];
        public int TenantId { get; set; }
        public string TenantName { get; set; }

        public List<int> SportIds { get; set; } = new();
        public List<int> FacilityIds { get; set; } = new();
        public List<int> BranchIds { get; set; } = new();
        public List<int> VenueIds { get; set; } = new();


        public List<AdminBranchDto> Branches { get; set; }

        public int Status { get; set; }
        public string? EntityType { get; set; }
        public Dashboard Dashboard { get; set; }
    }
    
    public class AdminBranchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
