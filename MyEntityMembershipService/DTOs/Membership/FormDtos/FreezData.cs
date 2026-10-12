namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class FreezData
    {
        public int MembershipId {  get; set; }
        public int FreezDuration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
