using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.MembershipDtos
{
    public class TenantMemberDto
    {
        public int? Id { get; set; }

        public string? Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MembershipStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string UserID { get; set; }

        public int TenantId { get; set; }
        public string BeltName { get; set; }
        public string? FileNumber { get; set; }
    }
}
