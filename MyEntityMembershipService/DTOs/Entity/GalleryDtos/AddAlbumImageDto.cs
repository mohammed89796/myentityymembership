namespace MyEntityMembershipService.DTOs.Entity.Gallery;

public class AddAlbumImageDto
{
    public int AlbumId { get; set; }
    public List<IFormFile> Images { get; set; } = [];
}

public class AlbumQueryParameters
{
    public string? SearchTerm { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}