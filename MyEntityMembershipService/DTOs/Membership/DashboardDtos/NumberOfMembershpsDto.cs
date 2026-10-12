namespace MyEntityMembershipService.DTOs.Membership.DashboardDtos
{
    public class NumberOfMembershpsDto
    {
        public int MinMembershipCount { get; set; } 
        public int MaxMembershipCount { get; set; } 
        public int Interval { get; set; } // Month of the membership (1-12)
        public Dictionary<string, List<MembershipSummaryDto>>  PakageMembershipNumbers {  get; set; }


    }
}
