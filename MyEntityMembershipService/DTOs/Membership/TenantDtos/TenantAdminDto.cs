using MyEntityMembershipService.DTOs.Entity.CoachSlotDtos;

namespace MyEntityMembershipService.DTOs.Membership.TenantDtos
{
    public class TenantAdminDto
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
    }
}
