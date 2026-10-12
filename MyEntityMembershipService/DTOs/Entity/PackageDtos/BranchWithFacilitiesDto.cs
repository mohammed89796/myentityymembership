namespace MyEntityMembershipService.DTOs.Entity.PackageDtos
{
    public class BranchWithFacilitiesDto
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public List<FacilityWithVenuesDto> Facilities { get; set; } = new();
    }
}
