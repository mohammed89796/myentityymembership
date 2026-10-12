namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class CreationCheckDto
    {
        public int? BranchId { get; set; }
        public int? FacilityId { get; set; }
        public int? VenueId { get; set; }
        public int TenantId { get; set; }

    }
}
