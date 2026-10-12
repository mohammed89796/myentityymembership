using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.Gallery;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.GalleryService;

public interface IGalleryService
{
    Task<Response<string>> CreateAlbumAsync(CreateAlbumDto albumDto);
    Task<Response<string>> UpdateAlbumAsync(UpdateAlbumDto albumDto);
    Task<Response<string>> DeleteAlbumAsync(int albumId, int tenantId);
    Task<Response<PaginationToReturnDto<AlbumResponseDto>>> GetAllTenantAlbumsAsync(int tenantId, AlbumQueryParameters queryParameters);
    Task<Response<AlbumResponseDto>> GetAlbumByIdAsync(int albumId);
    Task<Response<string>> UploadImageAsync(AddAlbumImageDto imageDto);
    Task<Response<string>> DeleteImageAsync(DeleteAlbumImageDto imageDto);
    
    
}