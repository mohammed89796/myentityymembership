using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.DTOs.Entity.MobileDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.CommentRepo
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly EntityMembershipDbContext _context;

        public CommentRepository(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<int> GetCountOfCommentsOfPostAsync(int postId)
           => await _context.Set<Comment>().CountAsync(p => p.PostId == postId);

        public async Task<List<CommentDto>> GetCommentsWithReplies(int postId)
        {
            var topLevelComments = await _context.Comments
                .Where(c => c.PostId == postId && c.ParentCommentId == null)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    CreatedAt = c.CreationDate,
                    ModifiedAt = c.ModificationDate,
                    CreatedBy = c.CreatedBy,
                    PostId = c.PostId,
                    ParentCommentId = c.ParentCommentId,
                    Replies = new List<CommentDto>()
                })
                .ToListAsync();


            for (int i = 0; i < topLevelComments.Count; i++)
            {
                topLevelComments[i].Replies = await GetRepliesAsync(topLevelComments[i].Id);
            }


            return topLevelComments;
        }

        private async Task<List<CommentDto>> GetRepliesAsync(int commentId)
        {
            var replies = await _context.Comments
                .Where(c => c.ParentCommentId == commentId)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    CreatedAt = c.CreationDate,
                    ModifiedAt = c.ModificationDate,
                    CreatedBy = c.CreatedBy,
                    PostId = c.PostId,
                    ParentCommentId = c.ParentCommentId,
                    Replies = new List<CommentDto>()
                })
                .ToListAsync();


            for (int i = 0; i < replies.Count; i++)
            {
                replies[i].Replies = await GetRepliesAsync(replies[i].Id);
            }


            return replies;
        }

        public async Task<IReadOnlyList<Comment>> GetCommentAndRepliesByIdAsync(int commentId)
        {
            var comment = await _context.Comments
            .Where(c => c.Id == commentId)
            .FirstOrDefaultAsync();


            var commentIds = new List<Comment>();

            if (comment is not null)
                commentIds.Add(comment);
            else
                return new List<Comment>();

            var replies = await GetRepliesIds(comment.Id);

            commentIds.AddRange(replies);

            return commentIds;
        }

        private async Task<List<Comment>> GetRepliesIds(int commentId)
        {
            var commentIds = new List<Comment>();

            var replies = await _context.Comments.Where(c => c.ParentCommentId == commentId).ToListAsync();

            for (int i = 0; i < replies.Count; i++)
            {

                commentIds.Add(replies.ToList()[i]);

                var Ids = await GetRepliesIds(replies.ToList()[i].Id);

                commentIds.AddRange(Ids);

            }

            return commentIds;
        }
    }
}
