using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.MemberTypeRepo
{
    public class MemberTypeRepo : GenericRepository<MemberType>, IMemberTypeRepo
    {
        private readonly EntityMembershipDbContext context;

        public MemberTypeRepo(EntityMembershipDbContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<List<MemberType>> GetAllTypeExceptMain()
        {
            return await context.MemberType
                .Where(mt => !(mt.Id == 5 && mt.Name == "Main Member")) // Exclude if Id is 5 and Name is "Main Member"
                .Where(mt => !(mt.Id != 5 && mt.Name == "Main Member")) // Exclude others with Name "Main Member"
                .ToListAsync();
        }

        public async Task<List<MemberType>> GetMemberTypes(string? Name, int? requestTypeId)
        {
            if (requestTypeId is not null && (requestTypeId == 6 || requestTypeId == 17))//new - cancelation
                return await context.MemberType.Where(type => type.Id == 5).ToListAsync();
            else if (!string.IsNullOrEmpty(Name))
                return await context.MemberType.Where(type => type.Name.ToLower().Contains(Name.Trim().ToLower())).ToListAsync();
            else if (requestTypeId is not null && (requestTypeId == 18 || requestTypeId == 9 || requestTypeId == 16))
                return await context.MemberType.Where(type => type.Id != 5).ToListAsync();
                
            else
                return await context.MemberType.ToListAsync();
        }

        public async Task<MemberType> GetTypeByName(string Name)
           => await context.MemberType.FirstOrDefaultAsync(type => type.Name == Name);
    }
}
