namespace MyEntityMembershipService.Data;

public class Gallery : BaseEntity
{
    public int AlbumId { get; set; }
    public Album Album { get; set; }
    public string Image { get; set; }
}