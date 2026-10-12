using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.BookMarkRepo
{
    public class BookMarkRepository : GenericRepository<BookMark>, IBookMarkRepository
    {
        private readonly EntityMembershipDbContext _context;

        public BookMarkRepository(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<bool> IsPostBookMarkedByUserAsync(int postId, string userId)
            => await _context.BookMarks.AnyAsync(b => b.PostId == postId && b.UserId == userId);

        public async Task<IReadOnlyList<BookMark>> GetBookMarkedPostsOfUserAsync(string userId)
            => await _context.BookMarks.AsNoTracking().Where(b => b.UserId == userId).ToListAsync();

        public async Task<BookMark> GetBookMarkedPostOfUserAsync(string userId, int postId)
           => await _context.BookMarks.FirstOrDefaultAsync(b => b.PostId == postId && b.UserId == userId);

        public async Task<BookMark> GetPostBookMarkedPreviouslyByUserAsync(int postId, string userId)
           => await _context.BookMarks.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.PostId == postId && b.UserId == userId);
    }
}
