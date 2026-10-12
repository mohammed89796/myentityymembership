using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.FormUserResponseRepo
{
    public class FormUserResponseRepository : GenericRepository<FormUserResponse>, IFormUserResponseRepository
    {
        private readonly EntityMembershipDbContext context;

        public FormUserResponseRepository(EntityMembershipDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<FormUserResponse> GetFormUserResponse(int Id)
            => await context.FormUserResponse.Where(response => response.Id == Id)
                            .Include(response => response.FieldsResponse)
                            .ThenInclude(responseF => responseF.Field)
                            .FirstOrDefaultAsync();

        public async Task<FormUserResponse> GetFormUserResponseWithForm(int RequestId)
            => await context.FormUserResponse.Where(response => response.MembershipRequestId == RequestId)
                                             .Include(response => response.FieldsResponse)
                                             .ThenInclude(response => response.Field)
                                             .ThenInclude(field => field.FieldType)
                                             .Include(response => response.Form).FirstOrDefaultAsync();

        public async Task<FormUserResponse> GetResponseByRequestId(int RequestId)
            => await context.FormUserResponse.Where(response => response.MembershipRequestId == RequestId)
                                             .Include(response => response.FieldsResponse).FirstOrDefaultAsync();
    }
}
