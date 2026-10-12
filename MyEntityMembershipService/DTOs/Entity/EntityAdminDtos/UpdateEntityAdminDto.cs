using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Entity.CoachSlotDtos;

namespace MyEntityMembershipService.DTOs.Entity.EntityAdminDtos
{
    public class UpdateEntityAdminDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public List<string>? RoleIds { get; set; } = new List<string>();
        public int TenantId { get; set; }
        public List<int>? BranchIds { get; set; }

        public List<int>? SportsIds { get; set; } = [];
        public List<int>? BranchsIds { get; set; } = [];

        public List<int>? VenuesIds { get; set; } = [];
        public List<int>? FacilitiesIds { get; set; } = [];
        public UpdateCoachSlotDto? CoachSlots { get; set; } 


    }
}
