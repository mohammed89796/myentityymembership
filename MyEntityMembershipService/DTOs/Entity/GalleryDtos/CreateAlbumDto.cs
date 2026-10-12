namespace MyEntityMembershipService.DTOs.Entity.Gallery;

public class CreateAlbumDto
{
    public string Title { get; set; }
    public List<IFormFile> Images { get; set; }
    public int TenantId { get; set; }
}