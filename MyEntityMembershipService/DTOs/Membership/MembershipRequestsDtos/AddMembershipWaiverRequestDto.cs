namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class AddMembershipWaiverRequestDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TenantId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SecondPhoneNumber { get; set; }
        public string? SecondEmail { get; set; }
        public int? BuildUpId { get; set; }
        public int MembershipId { get; set; }
    }
}
