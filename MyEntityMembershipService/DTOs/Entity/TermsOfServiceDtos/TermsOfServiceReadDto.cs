namespace MyEntityMembershipService.DTOs.Entity.TermsOfServiceDtos;

public class TermsOfServiceReadDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Terms { get; set; }
    public string? TermsUrl { get; set; }
    public int TenantId { get; set; }
    
}