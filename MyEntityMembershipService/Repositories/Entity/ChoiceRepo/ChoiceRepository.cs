using MyEntityMembershipService.Data;

using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.ChoiceRepo
{
    public class ChoiceRepository : GenericRepository<Choice>, IChoiceRepository
    {
        private readonly EntityMembershipDbContext _context;

        public ChoiceRepository(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<IReadOnlyList<Choice>> GetChoicesByIdsAsync(List<int> Ids)
        //    => await _context.Choices
    }
}
