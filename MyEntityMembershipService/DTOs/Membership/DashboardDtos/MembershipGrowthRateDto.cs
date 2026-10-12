namespace MyEntityMembershipService.DTOs.Membership.DashboardDtos
{

    public class MembershipGrowthRateDto
    {

        public int MinMembershipCount { get; set; }
        public int MaxMembershipCount { get; set; }
        public int Interval { get; set; } // Month of the membership (1-12)
        public Dictionary<string, List<MembershipGrowthRateSummaryDto>> PackageGrowthRates { get; set; }


        public class MembershipGrowthRateSummaryDto
        {
            public int PackageId { get; set; }
            public int Month { get; set; }
            public string MonthName { get; set; }
            public double GrowthRate { get; set; } // Growth rate for the specific month and package
            public int Year { get; set; }
        }
    }
}