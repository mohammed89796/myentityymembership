namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class PreviousFreezRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int FreezDuration { get; set; }
    }
}
