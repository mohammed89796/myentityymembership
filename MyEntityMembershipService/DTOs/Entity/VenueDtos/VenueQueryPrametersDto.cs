namespace MyEntityMembershipService.DTOs.Entity.VenueDtos;

public class VenueQueryPrametersDto
{
    public int? TenantId { get; set; }
    public int? BranchId { get; set; }
    public int? FacilityId { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}