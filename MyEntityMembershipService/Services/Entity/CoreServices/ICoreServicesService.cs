
using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.CoreServices
{
    public interface ICoreServicesService
    {
        Task<Response<bool>> CreateCoreService(string serviceName);
        Task<Response<List<CoreService>>> GetCoreServicesDropDown();

    }
}
