namespace MyEntityMembershipService.DTOs.Membership.DashboardDtos
{
    public class MembershipSummaryDto
    {
        public int PackageId { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }
        public int MembershipCount { get; set; }
    }

}
