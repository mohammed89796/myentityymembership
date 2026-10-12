namespace MyEntityMembershipService.DTOs.Entity.Gallery;

public class AlbumResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<ImageResponseDto> ImagesUrl { get; set; }
}
public class ImageResponseDto
{
    public int Id { get; set; }
    public string Url { get; set; }
}