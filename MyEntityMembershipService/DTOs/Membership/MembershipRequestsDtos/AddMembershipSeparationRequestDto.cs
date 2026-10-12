namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class AddMembershipSeparationRequestDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int TenantId { get; set; }
        public string? SeparationReason { get; set; }
        public string UserId { get; set; }
        public int MembershipId { get; set; }
    }
}
