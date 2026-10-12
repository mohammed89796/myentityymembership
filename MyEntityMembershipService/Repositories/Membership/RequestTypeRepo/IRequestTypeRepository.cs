using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.RequestTypeRepo
{
    public interface IRequestTypeRepository : IGenericRepository<RequestType>
    {
        Task<RequestType> GetTypeByName(string Name);
        Task<RequestType> GetRequestTypeWithFields(int Id);
        Task<List<RequestType>> GetAllTypeExceptNew();
        Task<RequestType> GetRequestTypeByNameAsync(string Name);

    }
}
