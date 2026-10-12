using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.Gallery;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.GalleryService;

namespace MyEntityMembershipService.Controllers.V1.Entity;
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize] 
public class GalleryController : ControllerBase
{
    private readonly IGalleryService _service;

    public GalleryController(IGalleryService service)
    {
        _service = service;
    }

    [HttpPost("albums")]
    [Consumes("multipart/form-data")]
    public async Task<IResult> CreateAlbum([FromForm] CreateAlbumDto dto)
    {
        int.TryParse(User.GetTenantId(), out var tenantId);
        if (tenantId > 0)
            dto.TenantId = tenantId;

        var result = await _service.CreateAlbumAsync(dto);
        return result.GetResponse();
    }

    [HttpPut("albums")]
    public async Task<IResult> UpdateAlbum([FromBody] UpdateAlbumDto dto)
    {
        int.TryParse(User.GetTenantId(), out var tenantId);
        if (tenantId > 0)
            dto.TenantId = tenantId;

        var result = await _service.UpdateAlbumAsync(dto);
        return result.GetResponse();
    }

    [HttpDelete("albums/{albumId:int}")]
    public async Task<IResult> DeleteAlbum([FromRoute] int albumId)
    {
        int.TryParse(User.GetTenantId(), out var tenantId);
        if (tenantId <= 0)
            return Results.Unauthorized();

        var result = await _service.DeleteAlbumAsync(albumId, tenantId);
        return result.GetResponse();
    }

    [HttpGet("albums/tenant/{tenantId:int}")]
    public async Task<IResult> GetAllTenantAlbums(int tenantId,[FromQuery] AlbumQueryParameters query)
    {
        var result = await _service.GetAllTenantAlbumsAsync(tenantId,query);
        return result.GetResponse();
    }

    [HttpGet("albums/{albumId:int}")]
    public async Task<IResult> GetAlbumById([FromRoute] int albumId)
    {
        var result = await _service.GetAlbumByIdAsync(albumId);
        return result.GetResponse();
    }

    [HttpPost("albums/images")]
    [Consumes("multipart/form-data")]
    public async Task<IResult> UploadImage([FromForm] AddAlbumImageDto dto)
    {
        var result = await _service.UploadImageAsync(dto);
        return result.GetResponse();
    }

    [HttpDelete("albums/images")]
    public async Task<IResult> DeleteImage([FromBody] DeleteAlbumImageDto dto)
    {
        var result = await _service.DeleteImageAsync(dto);
        return result.GetResponse();
    }
}