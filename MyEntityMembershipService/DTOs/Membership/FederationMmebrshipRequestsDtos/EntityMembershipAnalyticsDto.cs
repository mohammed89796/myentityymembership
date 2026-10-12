namespace MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos
{
    public class EntityMembershipAnalyticsDto
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int ExpiredMembers { get; set; }
        public string CreationDate { get; set; }

    }
}
