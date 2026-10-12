using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.RequestTypeServ
{
    public interface IRequestTypeService
    {
        Task<Response<List<RequestTypeDto>>> GetAllRequestTypes();
        Task<Response<RequestTypeDto>> GetRequestTypeById(int Id);
        Task<Response<RequestTypeDto>> AddRequestType(RequestTypeDto requestTypeDto);
        Task<Response<RequestTypeDto>> UpdateRequestType(RequestTypeDto requestTypeDto);
        Task<Response<RequestTypeDto>> DeleteRequestTypeById(int Id);
        Task<Response<List<RequestTypeDto>>> GetAllTypeExceptNew();
    }
}
