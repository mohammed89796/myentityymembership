namespace MyEntityMembershipService.DTOs.Entity.PackageDtos
{
    public class PackageSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }
        public double Price { get; set; }
        public int? Duration { get; set; }

        public int BranchCount { get; set; }
        public int FacilityCount { get; set; }
        public int VenueCount { get; set; }
    }

}
