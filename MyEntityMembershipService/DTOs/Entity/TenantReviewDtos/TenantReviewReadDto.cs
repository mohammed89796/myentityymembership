namespace MyEntityMembershipService.DTOs.Entity.TenantReviewDtos;

public class TenantReviewReadDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
