namespace MyEntityMembershipService.DTOs.Entity.TermsOfServiceDtos;

public class TermsOfServiceQueryParameters
{
    public int TenantId { get; set; }
    public string? SearchTerm { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}