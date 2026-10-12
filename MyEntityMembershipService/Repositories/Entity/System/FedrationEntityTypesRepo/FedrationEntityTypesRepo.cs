using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Entity.SystemDtos.FedrationEntityTypes;


using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.System.FedrationEntityTypesRepo
{
    public class FedrationEntityTypesRepo : GenericRepository<FedrationEntityType>, IFedrationEntityTypesRepo
    {
        public DbSet<FedrationEntityType> dbSet;
        private readonly EntityMembershipDbContext _context;

        public FedrationEntityTypesRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
            dbSet = context.Set<FedrationEntityType>();

        }

        public async Task<List<FedrationEntityTypesViewModel>> GetFedEntityTypes()
        {

            var response = await dbSet
                .Select(fb => new FedrationEntityTypesViewModel
                {
                    ID = fb.Id, 
                    Name = fb.Name
                })
                .ToListAsync();

            return response;
        }

    }
}
