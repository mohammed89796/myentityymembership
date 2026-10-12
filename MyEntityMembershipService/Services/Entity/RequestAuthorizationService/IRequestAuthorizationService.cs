using MyEntityMembershipService.DTOs.Entity.PackageDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.RequestAuthorizationService
{
    public interface IRequestAuthorizationService
    {
        Task<Response<RequestAuthorizationDto>> AddRequestAuthorization(RequestAuthorizationDto requestAuthorizationDto);
        Task<Response<List<RequestAuthorizationDto>>> GetAllRequestAuthorizations();
    }
}
