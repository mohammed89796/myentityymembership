
using MyEntityMembershipService.DTOs.Entity.PostDtos;
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Repositories.Entity.PostRepo
{
    public interface IPostRepo : IGenericRepository<Post>
    {
        //Task<Post> GetPostByIdAndDateAsync(int postId, DateOnly date);

        //Task<IReadOnlyList<Post>> GetPostsByTenantIdWithPaginationAsync(int tenantId, int pageNumber, int pageSize);

        //Task<IReadOnlyList<Post>> GetPostsByTenantIdWithPaginationAsync(int tenantId, int pageNumber, int pageSize, DateOnly date);

        //Task<IReadOnlyList<Post>> GetPostsByTenantIdAndUserIdPaginationAsync(SavedPostsOfUserDto dto);

        //Task<Post> GetPostWithSavedBookMarkByIdAsync(int postId, string userId, DateOnly date);

        Task<IReadOnlyList<Post>> GetPinnedPostsAsync(int tenantId);
        Task<int> GetCountOfPostsByTenantIdAsync(Expression<Func<Post, bool>> criteria);

        Task<Post> GetPostWithLikesAsync(int postId, string userId);

        Task<IReadOnlyList<object>> GetAllPosts(GetAllPostsWebDto dto);

        Task<Post> GetPostByIdAsyncAndIsNotDraft(int postId);

        Task<IReadOnlyList<object>> GetPostsOfOtherEntitiesAsync(int tenantId);

        Task<object> GetAnnouncementPostByIdAsync(int postId);

        Task<object> GetNewsFeedPostByIdAsync(int postId);

        Task<object> GetEventPollByIdAsync(int postId);

        Task<object> GetHighLightsByIdAsync(int postId);

        Task<IReadOnlyList<object>> GetAllMobilePosts(GetAllMobilePostsDto dto, DateOnly date);


    }
}
