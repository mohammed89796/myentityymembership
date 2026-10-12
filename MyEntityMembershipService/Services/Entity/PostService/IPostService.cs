
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.PostDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.PostService
{
    public interface IPostService
    {
        //Task<Response<PostToReturnDto>> CreatePost(PostDto postDto);

        //Task<Response<PaginationToReturnDto<PostToReturnDto>>> GetPostsByTenantIdWithPagination(int tenantId, int pageNumber, int pageSize);

        //Task<Response<PaginationToReturnDto<MobilePostToReturnDto>>> GetPostsForMobileByTenantIdWithPagination(GetAllPostsDto dto);

        //Task<Response<PostWithCommentsToReturnDto>> GetPostWithComments(int postId, string userId);

        //Task<Response<string>> BookMarkPost(int postId, string userId);

        //Task<Response<string>> UnBookMarkPost(int postId, string userId);

        Task<Post> GetPostByIdAsyncAndIsNotDraft(int postId);

        Task<Response<AnnouncementToReturnDto>> CreateAnnouncementPost(AnnouncementDto dto);

        Task<Response<NewsFeedToReturnDto>> CreateNewsFeedPost(NewsFeedDto dto);

        Task<Response<PollToReturnDto>> CreatePollPost(PollDto dto);

        Task<Response<HightlightToReturnDto>> CreateHighlightPost(HighlightDto dto);

        Task<Response<PaginationToReturnDto<object>>> GetAllPosts(GetAllPostsWebDto dto);

        Task<Response<PaginationToReturnDto<object>>> GetAllMobilePosts(GetAllMobilePostsDto dto);

        Task<Response<int>> LikeUnLikePost(int postId, string userId);

        Task<Response<IReadOnlyList<object>>> GetPostsOfOtherEntities(int tenantId);

        Task<Response<object>> GetPostByIdAsync(int postId);

        Task<Response<int>> SharePost(int postId, string userId);

    }
}
