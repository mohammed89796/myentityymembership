using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.FieldRepo
{
    public interface IFieldRepository : IGenericRepository<Field>
    {
        Task<Field> GetFieldWithDropdownValues(int fieldId);
        Task<List<Field>> GetAllFields();
    }
}
