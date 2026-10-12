using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.Gallery;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;

namespace MyEntityMembershipService.Services.Entity.GalleryService;

public class GalleryService : IGalleryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ResponseHandler _rh = new();

    public GalleryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Response<string>> CreateAlbumAsync(CreateAlbumDto albumDto)
    {
        try
        {
            if (albumDto is null)
                return _rh.BadRequest<string>("Invalid payload");

           
            var imageUrls = await UploadManyAsync(albumDto.Images);

            var album = new Album
            {
                Title = albumDto.Title,
                TenantId = albumDto.TenantId,
                Galleries = imageUrls.Select(url => new Gallery { Image = url }).ToList()
            };

            await _unitOfWork.Repository<Album>().Add(album);
            await _unitOfWork.Save();

            return _rh.Success("Album Created Successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in CreateAlbumAsync");
            return _rh.BadRequest<string>("Something went wrong");
        }
    }

    public async Task<Response<string>> UpdateAlbumAsync(UpdateAlbumDto albumDto)
    {
        try
        {
            var specs = new GeneralSpecifications<Album>(a => a.Id == albumDto.Id && a.TenantId == albumDto.TenantId);
            var album = await _unitOfWork.Repository<Album>().GetWithSpecifications(specs);

            if (album is null)
                return _rh.BadRequest<string>("Album not found");

            album.Title = albumDto.Title;

            _unitOfWork.Repository<Album>().Update(album);
            await _unitOfWork.Save();

            return _rh.Success("Album Updated Successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in UpdateAlbumAsync");
            return _rh.BadRequest<string>("Something went wrong");
        }
    }

    public async Task<Response<string>> DeleteAlbumAsync(int albumId, int tenantId)
    {
        try
        {
            var specs = new GeneralSpecifications<Album>(a => a.Id == albumId && a.TenantId == tenantId);
            specs.AddInclude(q => q.Include(a => a.Galleries));

            var album = await _unitOfWork.Repository<Album>().GetWithSpecifications(specs);
            if (album is null)
                return _rh.BadRequest<string>("Album not found");

            _unitOfWork.Repository<Album>().Delete(album);
            await _unitOfWork.Save();

            return _rh.Success("Album Deleted Successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in DeleteAlbumAsync");
            return _rh.BadRequest<string>("Something went wrong");
        }
    }

    public async Task<Response<PaginationToReturnDto<AlbumResponseDto>>> GetAllTenantAlbumsAsync(
        int tenantId,
        AlbumQueryParameters queryParameters)
    {
        try
        {
            queryParameters ??= new AlbumQueryParameters();

            var term =  queryParameters?.SearchTerm?.ToLower().Trim();
            var termLower = term?.ToLower();

            var specs = new GeneralSpecifications<Album>(
                a => a.TenantId == tenantId &&
                     (string.IsNullOrWhiteSpace(termLower) ||
                      a.Title.ToLower().Trim().Contains(termLower)),
                queryParameters.PageIndex,
                queryParameters.PageSize
            );

            // If your projection sometimes fails to translate nested collections, add include:
            // specs.AddInclude(q => q.Include(a => a.Galleries));

            var albums = await _unitOfWork.Repository<Album>()
                .GetAllWithSpecificationsAndProjection(specs, a => new AlbumResponseDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ImagesUrl = a.Galleries
                        .OrderByDescending(g => g.Id)
                        .Take(5)
                        .Select(g => new ImageResponseDto()
                        {
                            Id = g.Id,
                            Url = g.Image
                        })
                        .ToList()
                });

            var count = await _unitOfWork.Repository<Album>().GetCountByCriteriaAsync(a =>
                a.TenantId == tenantId &&
                (string.IsNullOrWhiteSpace(termLower) ||
                 a.Title.ToLower().Trim().Contains(termLower))
            );

            await Task.WhenAll(albums.Select(async album =>
            {
                if (album.ImagesUrl is null || album.ImagesUrl.Count == 0)
                    return;

                var presigned = await Task.WhenAll(album.ImagesUrl.Select(async img =>
                {
                    if (string.IsNullOrWhiteSpace(img.Url))
                        return img;

                    var presignedUrl = await DocumentSetting.GetPreSignedUrlAsync(img.Url);

                    return new ImageResponseDto
                    {
                        Id = img.Id,
                        Url = presignedUrl
                    };
                }));

                album.ImagesUrl = presigned.ToList();
            }));
            var totalPages = CalcTotalPages(count, queryParameters.PageSize);

            var response = new PaginationToReturnDto<AlbumResponseDto>(
                count,
                totalPages,
                queryParameters.PageSize,
                queryParameters.PageIndex,
                albums
            );

            return _rh.Success(response);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetAllTenantAlbumsAsync");
            return _rh.BadRequest<PaginationToReturnDto<AlbumResponseDto>>("Something went wrong");
        }
    }

    public async Task<Response<AlbumResponseDto>> GetAlbumByIdAsync(int albumId)
    {
        try
        {
            var specs = new GeneralSpecifications<Album>(a => a.Id == albumId);
            specs.AddInclude(q => q.Include(a => a.Galleries));

            var album = await _unitOfWork.Repository<Album>()
                .GetWithSpecificationsAndProjection(specs, a => new AlbumResponseDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ImagesUrl = a.Galleries
                        .OrderByDescending(g => g.Id)
                        .Select(g => new ImageResponseDto()
                        {
                            Id = g.Id,
                            Url = g.Image
                        })
                        .ToList()
                });

            if (album is null)
                return _rh.BadRequest<AlbumResponseDto>("Album not found");

            if (album.ImagesUrl is not null && album.ImagesUrl.Count > 0)
            {
                var imagesWithPresigned = await Task.WhenAll(
                    album.ImagesUrl.Select(async img =>
                    {
                        if (string.IsNullOrWhiteSpace(img.Url))
                            return img;

                        var presignedUrl = await DocumentSetting.GetPreSignedUrlAsync(img.Url);

                        return new ImageResponseDto
                        {
                            Id = img.Id,
                            Url = presignedUrl
                        };
                    })
                );

                album.ImagesUrl = imagesWithPresigned.ToList();
            }

            return _rh.Success(album);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in GetAlbumByIdAsync : {AlbumId}", albumId);
            return _rh.BadRequest<AlbumResponseDto>("Something went wrong");
        }
    }

    public async Task<Response<string>> UploadImageAsync(AddAlbumImageDto imageDto)
    {
        try
        {
            if (imageDto is null || imageDto.AlbumId <= 0 || imageDto.Images.Count == 0)
                return _rh.BadRequest<string>("Invalid payload");

            var specs = new GeneralSpecifications<Album>(a => a.Id == imageDto.AlbumId);
            var album = await _unitOfWork.Repository<Album>().GetWithSpecifications(specs);

            if (album is null)
                return _rh.BadRequest<string>("Album not found");

            var albumImages = new List<Gallery>();
            foreach (var image in imageDto.Images)
            {
                var imageUrl = await DocumentSetting.AddToContainerAndGetUrl(null, null, image);

                var albumImage = new Gallery
                {
                    Image = imageUrl,
                    AlbumId = imageDto.AlbumId
                };
                albumImages.Add(albumImage);
            }

            _unitOfWork.Repository<Gallery>().AddRange(albumImages);
            await _unitOfWork.Save();

            return _rh.Success("Image Uploaded Successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in UploadImageAsync : {AlbumId}", imageDto?.AlbumId ?? 0);
            return _rh.BadRequest<string>("Something went wrong");
        }
    }

    public async Task<Response<string>> DeleteImageAsync(DeleteAlbumImageDto imageDto)
    {
        try
        {
            if (imageDto is null || imageDto.AlbumId <= 0 || imageDto.ImageId <= 0)
                return _rh.BadRequest<string>("Invalid payload");

            var specs = new GeneralSpecifications<Gallery>(g => g.Id == imageDto.ImageId && g.AlbumId == imageDto.AlbumId);
            var gallery = await _unitOfWork.Repository<Gallery>().GetWithSpecifications(specs);

            if (gallery is null)
                return _rh.BadRequest<string>("Image Not Found in this album");


            _unitOfWork.Repository<Gallery>().Delete(gallery);
            await _unitOfWork.Save();

            return _rh.Success("Image Deleted Successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in DeleteImageAsync : imageId={ImageId}, albumId={AlbumId}",
                imageDto?.ImageId ?? 0, imageDto?.AlbumId ?? 0);

            return _rh.BadRequest<string>("Something went wrong");
        }
    }
    
    private static int CalcTotalPages(int count, int pageSize)
    {
        if (count <= 0 || pageSize <= 0) return 0;
        return (int)Math.Ceiling(count / (double)pageSize);
    }

    private static async Task<List<string>> UploadManyAsync(IEnumerable<IFormFile>? files)
    {
        if (files is null) return new List<string>();

        var list = files.Where(f => f != null).ToList();
        if (list.Count == 0) return new List<string>();

        var uploadTasks = list
            .Select(f => DocumentSetting.AddToContainerAndGetUrl(null, null, file: f))
            .ToArray();

        var results = await Task.WhenAll(uploadTasks);
        return results.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
    }

    private static async Task ReplaceWithPresignedUrlsAsync(List<string> urls)
    {
        var task = urls.Select(async utl => utl = await DocumentSetting.GetPreSignedUrlAsync(utl));
        await Task.WhenAll(task);
        
    }
}