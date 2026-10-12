using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.UnitRepo
{
    public interface IUnitRepository : IGenericRepository<Unit>
    {
        Task<Unit> GetByIdWithUnitTypeAsync(int id);
        Task<Unit> GetByNameWithUnitTypeAsync(string name);
        Task<List<Unit>> GetAllWithUnitTypeAsync();
        Task<List<Unit>> GetByUnitTypeIdWithTypeNameAsync(int unitTypeId);
    }
}
