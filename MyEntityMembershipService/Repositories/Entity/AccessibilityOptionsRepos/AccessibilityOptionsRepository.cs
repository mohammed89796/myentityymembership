using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.AccessibilityOptionsRepos
{
    public class AccessibilityOptionsRepository : GenericRepository<NewAccessibilityOptions>, IAccessibilityOptionsRepository
    {
        private readonly EntityMembershipDbContext _context;

        public AccessibilityOptionsRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<NewAccessibilityOptions>> GetAllAccessibilityOptionsAsync()
        {
            return await _context.NewAccessibilityOptions.ToListAsync();
        }
    }
}
