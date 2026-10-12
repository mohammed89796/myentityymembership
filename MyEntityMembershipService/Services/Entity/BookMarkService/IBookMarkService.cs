

namespace MyEntityMembershipService.Services.Entity.BookMarkService
{
    public interface IBookMarkService
    {
        Task<bool> IsPostBookMarkedByUser(int postId, string userId);

        Task<IReadOnlyList<BookMark>> GetBookMarkedPostsOfUser(string userId);

        Task<BookMark> GetBookMarkedPostOfUserAsync(string userId, int postId);

        Task<BookMark> IsPostBookMarkedPreviouslyByUser(int postId, string userId);
    }
}
