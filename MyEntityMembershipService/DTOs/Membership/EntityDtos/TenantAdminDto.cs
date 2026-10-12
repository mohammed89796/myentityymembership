using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity.CoachSlotDtos;

namespace MyEntityMembershipService.DTOs.Membership.EntityDtos
{
    public class TenantAdminDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserId { get; set; }
        public EntityAdminStatus EntityAdminStatus { get; set; }

        public int TenantId { get; set; }
        public string? TenantName { get; set; } 

        public List<int> AreaIds { get; set; } = new();
        public List<int> RoleIds { get; set; } = new();
        public List<int> SportIds { get; set; } = new();
        public List<int> FacilityIds { get; set; } = new();
        public List<int> BranchIds { get; set; } = new();
        public List<int> VenueIds { get; set; } = new();
        public List<CoachSlotDto> CoachSlots { get; set; }=new();

    }
}
