
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.BookMarkRepo
{
    public interface IBookMarkRepository : IGenericRepository<BookMark>
    {
        Task<bool> IsPostBookMarkedByUserAsync(int postId, string userId);

        Task<IReadOnlyList<BookMark>> GetBookMarkedPostsOfUserAsync(string userId);

        Task<BookMark> GetBookMarkedPostOfUserAsync(string userId, int postId);

        Task<BookMark> GetPostBookMarkedPreviouslyByUserAsync(int postId, string userId);


    }
}
