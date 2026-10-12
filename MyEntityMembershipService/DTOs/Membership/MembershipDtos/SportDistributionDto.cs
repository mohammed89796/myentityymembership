namespace MyEntityMembershipService.DTOs.Membership.MembershipDtos
{
    public class SportDistributionDto
    {
        public int TotalAthletes { get; set; }
        public List<SportAthleteCountDto> Sports { get; set; }
    }

    public class SportAthleteCountDto
    {
        public int SportId { get; set; }
        public string SportName { get; set; }
        public string SportNameAr { get; set; }
        public int AthleteCount { get; set; }
    }


    public class SportDistributionFilterDto
    {
       public int? Year { get; set; }
    }
}
