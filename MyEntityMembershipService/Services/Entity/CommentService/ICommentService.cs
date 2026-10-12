using MyEntityMembershipService.DTOs.Entity.MobileDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.CommentService
{
    public interface ICommentService
    {
        Task<Response<object>> AddComment(AddCommentDto dto);

        Task<Response<object>> EditComment(int commentId, string userId, string text);

        Task<Response<int>> DeleteComment(int commentId, string userId);

        Task<Response<List<CommentDto>>> GetCommentsWithReplies(int postId);

        Task<int> GetCountOfCommentsOfPost(int postId);
    }
}
