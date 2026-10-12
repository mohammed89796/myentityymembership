namespace MyEntityMembershipService.DTOs.Membership.TenantDtos
{
    public class TenantReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        public string? Image { get; set; }

    }
}
