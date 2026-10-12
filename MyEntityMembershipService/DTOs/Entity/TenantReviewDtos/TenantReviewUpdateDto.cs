namespace MyEntityMembershipService.DTOs.Entity.TenantReviewDtos;

public class TenantReviewUpdateDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public IFormFile? ImageFile { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
