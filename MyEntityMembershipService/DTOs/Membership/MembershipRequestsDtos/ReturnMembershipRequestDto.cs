namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class ReturnMembershipRequestDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? NationalId { get; set; }
        public string? NationalIdPhoto { get; set; }
        public string? UserPhoto { get; set; }
        public int TenantId { get; set; }
        public string UserId { get; set; }
    }
}
