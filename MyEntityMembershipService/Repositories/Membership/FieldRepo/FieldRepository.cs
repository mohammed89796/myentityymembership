using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.FieldRepo
{
    public class FieldRepository : GenericRepository<Field>, IFieldRepository
    {
        private readonly EntityMembershipDbContext context;

        public FieldRepository(EntityMembershipDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Field>> GetAllFields()
            => await context.Fields.ToListAsync();

        public async Task<Field> GetFieldWithDropdownValues(int fieldId)
            => await context.Fields.Where(field => field.Id == fieldId).Include(field => field.FieldType)
                                   .Include(field => field.DropdownValues)
                                   .FirstOrDefaultAsync();
    }
}
