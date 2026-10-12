namespace MyEntityMembershipService.DTOs.Entity.TermsOfServiceDtos;

public class TermsOfServiceCreateDto
{
    public string Title { get; set; }
    public string? Terms { get; set; }
    public IFormFile? TermsFile { get; set; }
    public int TenantId { get; set; }
}