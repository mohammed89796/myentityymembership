namespace MyEntityMembershipService.DTOs.Entity.TermsOfServiceDtos;

public class TermsOfServiceUpdateDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Terms { get; set; }
    public IFormFile? TermsFile { get; set; }
    public int TenantId { get; set; }
}