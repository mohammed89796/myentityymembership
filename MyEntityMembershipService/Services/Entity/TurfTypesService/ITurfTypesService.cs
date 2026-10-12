

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.TurfTypesService
{
    public interface ITurfTypesService
    {
        Task<Response<bool>> CreateTurfType(string typeName);
        Task<Response<List<TurfTypes>>> GetTurfTypesDropDown();

    }
}
