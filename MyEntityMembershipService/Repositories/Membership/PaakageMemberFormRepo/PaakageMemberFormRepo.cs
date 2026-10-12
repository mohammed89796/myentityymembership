using MyEntityMembershipService.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyEntityMembershipService.Repositories.GenericRepo;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Repositories.Membership.PaakageMemberFormRepo
{
    public class PaakageMemberFormRepo : GenericRepository<PaakageMemberForm>, IPaakageMemberFormRepo
    {
        private readonly EntityMembershipDbContext context;

        public PaakageMemberFormRepo(EntityMembershipDbContext context) : base(context)
        {
            this.context = context;
        }

        // Get all PaakageMemberForms except a certain MemberType
        public async Task<List<PaakageMemberForm>> GetAllForms()
        {
            return await context.PaakageMemberForms
                .Include(f=>f.Form)
                .ToListAsync();
        }
        public async Task<List<PaakageMemberForm>> GetAllFormsWithoughtNew()
        {
            return await context.PaakageMemberForms
                .Include(f => f.Form)
                .Where(f => f.Form.RequestTypeId != 6)
                .ToListAsync();
        }

        public async Task<PaakageMemberForm> GetMemberFormByFormIdPackageIdmemberTypeId(int packageId, int formId, int memberTypeId)
            => await context.PaakageMemberForms.FirstOrDefaultAsync(p => p.PackageId == packageId && p.FormId == formId && p.MemberTypeId == memberTypeId);
    }
}
