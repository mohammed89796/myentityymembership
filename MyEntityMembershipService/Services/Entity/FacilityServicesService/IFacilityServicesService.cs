
using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.FacilityServicesService
{
    public interface IFacilityServicesService
    {
        Task<Response<bool>> CreateFacilityService(string serviceName);
        Task<Response<List<CoreFacilityServices>>> GetFacilityServicesDropDown();

    }
}
