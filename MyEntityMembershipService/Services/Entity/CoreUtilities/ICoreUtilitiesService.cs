

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.CoreUtilities
{
    public interface ICoreUtilitiesService
    {
        Task<Response<bool>> CreateCoreUtilitie(string typeName);
        Task<Response<List<NewCoreUtilities>>> GetCoreUtilitiesDropDown();

    }
}
