using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.MemberTypeRepo
{
    public interface IMemberTypeRepo : IGenericRepository<MemberType>
    {
        Task<List<MemberType>> GetAllTypeExceptMain();
        Task<MemberType> GetTypeByName(string Name);
        Task<List<MemberType>> GetMemberTypes(string? Name, int? requestTypeId);
    }
}
