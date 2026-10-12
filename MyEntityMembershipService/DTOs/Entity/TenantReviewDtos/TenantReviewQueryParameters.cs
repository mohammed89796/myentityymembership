namespace MyEntityMembershipService.DTOs.Entity.TenantReviewDtos;

public class TenantReviewQueryParameters
{
    public int TenantId { get; set; }
    public string? SearchTerm { get; set; }
    public int? Rating { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
