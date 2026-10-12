namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class AddMembershipRequestDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TenantId { get; set; }
        public int MembershipId { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
