
using MyEntityMembershipService.Repositories.Entity.BookMarkRepo;

namespace MyEntityMembershipService.Services.Entity.BookMarkService
{
    public class BookMarkService : IBookMarkService
    {
        private readonly IBookMarkRepository _bookMarkRepository;

        public BookMarkService(IBookMarkRepository bookMarkRepository)
        {
            _bookMarkRepository = bookMarkRepository;
        }
        public async Task<bool> IsPostBookMarkedByUser(int postId, string userId)
            => await _bookMarkRepository.IsPostBookMarkedByUserAsync(postId, userId);

        public async Task<BookMark> IsPostBookMarkedPreviouslyByUser(int postId, string userId)

             => await _bookMarkRepository.GetPostBookMarkedPreviouslyByUserAsync(postId, userId);


        public async Task<IReadOnlyList<BookMark>> GetBookMarkedPostsOfUser(string userId)
            => await _bookMarkRepository.GetBookMarkedPostsOfUserAsync(userId);

        public async Task<BookMark> GetBookMarkedPostOfUserAsync(string userId, int postId)
            => await _bookMarkRepository.GetBookMarkedPostOfUserAsync(userId, postId);
    }
}
