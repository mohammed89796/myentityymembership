using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;
using MyEntityMembershipService.Repositories.Membership.PackageRequestAuthourizationsRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Repositories.Membership.FormRepo
{
    public class FormRepository : GenericRepository<Form>, IFormRepository
    {
        private readonly EntityMembershipDbContext context;

        public FormRepository(EntityMembershipDbContext context, IPackageRequestAuthorizationRepository packageRequestAuthorizationRepository, IUnitOfWork unitOfWork) : base(context)
        {
            this.context = context;
        }

        public async Task<Form> GetForm(int id)
            => await context.Forms.Where(form => form.Id == id).Include(form => form.RequestType)
                                        .Include(form => form.MemberTypes)
                                        .Include(form => form.FormFields.OrderBy(o => o.Index)).FirstOrDefaultAsync();

        public async Task<List<Form>> GetFormsByTypeId(int requestTypeId,int tenantId)
            => await context.Forms.Where(form => form.RequestTypeId == requestTypeId && form.TenantId == tenantId).ToListAsync();

        public async Task<List<Form>> GetFormsCreatedBy(string userId, int requestTypeId, string? title)
        {
            if (!string.IsNullOrEmpty(title))
                return await context.Forms.Where(form => form.CreatedBy == userId && form.RequestTypeId == requestTypeId && form.Title.ToLower().Contains(title.Trim().ToLower()))
                                  .Include(form => form.RequestType).OrderByDescending(form => form.Id).ToListAsync();
            else
                return await context.Forms.Where(form => form.CreatedBy == userId && form.RequestTypeId == requestTypeId)
                                  .Include(form => form.RequestType).OrderByDescending(form => form.Id).ToListAsync();
        }

        public async Task<List<RequestTypeCategory>> GetRequestTypeCategories()
            => await context.RequestTypeCategory.Include(category => category.RequestTypes).ToListAsync();

        public async Task<List<Form>> GetFormsByTypeAndMemberTypeId(int memberTypeId, int requestTypeId)
        {
            return await context.Forms
                .Include(f => f.RequestType)
                .Include(f => f.MemberTypes)  // Include related MemberTypes
                .Where(f => f.RequestTypeId == requestTypeId && f.MemberTypes.Any(mt => mt.MemberTypeId == memberTypeId))
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Form>> GetNewRegistrationFormsByRequestIdAsync(int requestTypeId)
            => await context.Forms.AsNoTracking().Where(m => m.RequestTypeId == requestTypeId && m.IsDraft!=true).ToListAsync();

        public async Task<Form> GetNewRegistrationFormsByIdAsync(int Id)
                => await context.Forms.AsNoTracking().FirstOrDefaultAsync(m => m.Id == Id);

    }
}
