namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class SubmitCommentsAndJustficationsDto
    {
        public int tenantId { get; set; }
        public int requestId { get; set; }
        public string comments { get; set; }
        public string? CreatedBy { get; set; }
    }
}
