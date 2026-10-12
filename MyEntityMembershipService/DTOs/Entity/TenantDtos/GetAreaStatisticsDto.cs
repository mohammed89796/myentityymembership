namespace MyEntityMembershipService.DTOs.Entity.TenantDtos
{
    public class GetAreaStatisticsDto
    {
        public int NumberOfEntities { get; set; }
        public int NumberOfAcademies { get; set; }
        public int NumberOfClubs { get; set; }
        public int NumberOfYouthClubs { get; set; }
        public int NumberOfTrainingCenters { get; set; }
    }
}
