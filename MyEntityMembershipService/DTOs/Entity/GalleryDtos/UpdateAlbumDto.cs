namespace MyEntityMembershipService.DTOs.Entity.Gallery;

public class UpdateAlbumDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int TenantId { get; set; }
}