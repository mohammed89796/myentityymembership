
using MyEntityMembershipService.DTOs.Entity.MobileDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.CommentRepo
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<List<CommentDto>> GetCommentsWithReplies(int postId);

        Task<IReadOnlyList<Comment>> GetCommentAndRepliesByIdAsync(int commentId);

        Task<int> GetCountOfCommentsOfPostAsync(int postId);

    }
}
