using MyEntityMembershipService.Data;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Repositories.GenericRepo;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Repositories.Membership.RequestTypeRepo
{
    public class RequestTypeRepository : GenericRepository<RequestType>, IRequestTypeRepository
    {
        private readonly EntityMembershipDbContext context;

        public RequestTypeRepository(EntityMembershipDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<RequestType> GetTypeByName(string Name)
            => await context.RequestTypes.FirstOrDefaultAsync(type => type.Name == Name);

        public async Task<RequestType> GetRequestTypeWithFields(int Id)
            => await context.RequestTypes.Where(type => type.Id == Id).Include(type => type.Fields).FirstOrDefaultAsync();
        public async Task<List<RequestType>> GetAllTypeExceptNew()
        {
            return await context.RequestTypes
                .Where(mt => !(mt.Id == 6))
                .ToListAsync();
        }

        public async Task<RequestType> GetRequestTypeByNameAsync(string Name)
            => await context.RequestTypes.AsNoTracking().FirstOrDefaultAsync(type => type.Name.ToLower().Replace(" ","") == Name);
    }
}
