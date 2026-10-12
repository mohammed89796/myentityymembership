namespace MyEntityMembershipService.DTOs.Entity.PackageDtos
{
    public class GetPackageDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public int? Duration { get; set; }
        public double Price { get; set; }
        public int TenantId { get; set; }
        public int BranchesCount { get; set; }
        public int FacilitiesCount { get; set; }
        public int VenuesCount { get; set; }


    }
}
