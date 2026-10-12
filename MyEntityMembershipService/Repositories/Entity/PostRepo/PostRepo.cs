using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity.PostDtos;
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Repositories.Entity.PostRepo
{
    public class PostRepo : GenericRepository<Post>, IPostRepo
    {
        private readonly EntityMembershipDbContext _context;

        public PostRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<IReadOnlyList<Post>> GetPostsByTenantIdWithPaginationAsync(int tenantId, int pageNumber, int pageSize)

        //    => await _context.Set<Post>()
        //    .Where(p => p.TenantId == tenantId && p.PublishDate <= DateOnly.FromDateTime(DateTime.Now))
        //    .OrderByDescending(p => p.PublishDate)
        //    .Include(p => p.Tenant)
        //    .Include(p => p.PostTags)
        //    .Skip(pageSize * (pageNumber - 1))
        //    .Take(pageSize)
        //    .ToListAsync();

        //public async Task<IReadOnlyList<Post>> GetPostsByTenantIdWithPaginationAsync(int tenantId, int pageNumber, int pageSize, DateOnly date)

        //    => await _context.Set<Post>()
        //    .Where(p => p.TenantId == tenantId && p.PublishDate <= date)
        //    .OrderByDescending(p => p.PublishDate)
        //    .Include(p => p.Tenant)
        //    .Include(p => p.PostTags)
        //    .Skip(pageSize * (pageNumber - 1))
        //    .Take(pageSize)
        //    .ToListAsync();


        //public async Task<IReadOnlyList<Post>> GetPostsByTenantIdAndUserIdPaginationAsync(SavedPostsOfUserDto dto)

        //   => await _context.Set<Post>()
        //   .Where(p => p.TenantId == dto.TenantId && p.PublishDate <= dto.Date)
        //   .OrderByDescending(p => p.PublishDate)
        //   .Include(p => p.Tenant)
        //   .Include(p => p.BookMarks.Where(bm => bm.UserId == dto.UserId))
        //   .Skip(dto.PageSize * (dto.PageNumber - 1))
        //   .Take(dto.PageSize)
        //   .ToListAsync();

        //public async Task<Post> GetPostWithSavedBookMarkByIdAsync(int postId, string userId, DateOnly date)
        //     => await _context.Set<Post>()
        //   .Where(p => p.Id == postId && p.PublishDate <= date)
        //   .Include(p => p.Tenant)
        //   .Include(p => p.BookMarks.Where(bm => bm.UserId == userId))
        //   .FirstOrDefaultAsync();


        //public async Task<Post> GetPostByIdAndDateAsync(int postId, DateOnly date)
        //     => await _context.Set<Post>()
        //   .FirstOrDefaultAsync(p => p.Id == postId && p.PublishDate <= date);


        public async Task<int> GetCountOfPostsByTenantIdAsync(Expression<Func<Post, bool>> criteria)
            => await _context.Set<Post>().CountAsync(criteria);



        public async Task<Post> GetPostWithLikesAsync(int postId, string userId)
            => await _context.Posts.Where(p => p.Id == postId && p.IsDraft == false)
            .Include(p => p.Likes.Where(l => l.CreatedBy == userId))
            .FirstOrDefaultAsync();


        public async Task<IReadOnlyList<Post>> GetPinnedPostsAsync(int tenantId)
            => await _context.Posts
            .Where(p => p.IsPinned == true && p.TenantId == tenantId && p.IsDraft == false)
            .OrderByDescending(p => p.CreationDate)
            .ToListAsync();

        public async Task<Post> GetPostByIdAsyncAndIsNotDraft(int postId)
            => await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == postId && p.IsDraft == false);


        public async Task<IReadOnlyList<object>> GetPostsOfOtherEntitiesAsync(int tenantId)
            => await _context.Posts.AsNoTracking()
            .Where(p => p.TenantId != tenantId && p.IsDraft == false)
            .OrderBy(e => Guid.NewGuid())
            .Take(7)
            .Select(p => new
            {
                p.Id,
                p.PostTitle,
                p.Description,
                PostPictures = p.PostPictureUrl,
                p.TenantId,
                p.CreatedBy,
                p.PostType,
                p.PublishDate,
                p.CreationDate,
            }).ToListAsync();


        public async Task<object> GetAnnouncementPostByIdAsync(int postId)
            => await _context.Posts.AsNoTracking().Where(p => p.Id == postId && p.IsDraft == false)
            .Select(p => new
            {
                p.Id,
                p.PostTitle,
                p.Description,
                PostPictures = p.PostPictureUrl,
                p.CreationDate,
                p.PublishDate,
                p.TenantId,
                p.CreatedBy,
                PostType = PostType.Announcement,
                Priority = p.Priority.ToString(),
                p.IsPinned,
                p.IsDraft,
            }).FirstOrDefaultAsync();


        public async Task<object> GetNewsFeedPostByIdAsync(int postId)
           => await _context.Posts.AsNoTracking().Where(p => p.Id == postId && p.IsDraft == false)
           .Select(p => new
           {
               p.Id,
               p.PostTitle,
               p.Description,
               PostPictures = p.PostPictureUrl,
               p.CreationDate,
               p.PublishDate,
               p.TenantId,
               p.CreatedBy,
               PostType = PostType.NewsFeed,
               p.IsDraft,
               PostTags = p.PostTags.Select(pt => new { pt.Id, pt.Tag }).ToList(),
               p.HasComments,
               p.HasLikes,
               p.HasShares,
               NumberOfLikes = p.HasLikes ? p.Likes.Count : (int?)null,
               NumberOfComments = p.HasComments ? p.Comments.Count : (int?)null,
               NumberOfShares = p.HasShares ? p.Shares.Count : (int?)null
           }).FirstOrDefaultAsync();

        public async Task<object> GetEventPollByIdAsync(int postId)
           => await _context.Posts.AsNoTracking().Where(p => p.Id == postId && p.IsDraft == false)
           .Select(p => new
           {
               p.Id,
               p.PostTitle,

               p.CreationDate,
               p.PublishDate,
               p.TenantId,
               p.CreatedBy,
               PostType = PostType.EventPoll,
               p.IsDraft,
               HasMultiselect = p.Poll.HasMultiSelect,
               p.Poll.Question,
               PollId = p.Poll.Id,
               Choices = p.Poll.Choices.Select(c => new
               {
                   c.Id,
                   c.Text,
                   Votes = c.Poll.Choices.Sum(choice => choice.Vote) > 0
                                            ? (int)Math.Round((double)c.Vote / c.Poll.Choices.Sum(choice => choice.Vote) * 100)
                                            : 0
               }).ToList()
           }).FirstOrDefaultAsync();

        public async Task<object> GetHighLightsByIdAsync(int postId)
          => await _context.Posts.AsNoTracking().Where(p => p.Id == postId && p.IsDraft == false)
          .Select(p => new
          {
              p.Id,
              p.PostTitle,
              p.Description,
              PostPictures = p.PostPictureUrl,
              p.PublishDate,
              p.CreationDate,
              p.TenantId,
              p.CreatedBy,
              PostType = PostType.HighLights,
              p.IsDraft,
              p.HasComments,
              p.HasLikes,
              p.HasShares,
              NumberOfLikes = p.HasLikes ? p.Likes.Count : (int?)null,
              NumberOfComments = p.HasComments ? p.Comments.Count : (int?)null,
              NumberOfShares = p.HasShares ? p.Shares.Count : (int?)null
          }).FirstOrDefaultAsync();


        public async Task<IReadOnlyList<object>> GetAllPosts(GetAllPostsWebDto dto)
        {
            var posts = await _context.Posts
                .AsNoTracking()
                .Where(p => p.TenantId == dto.TenantId && !p.IsDraft && (string.IsNullOrEmpty(dto.SearchTerm) || p.PostTitle.ToLower().Replace(" ", "").Contains(dto.SearchTerm) || p.Poll.Question.ToLower().Replace(" ", "").Contains(dto.SearchTerm)))
                .OrderByDescending(p => p.IsPinned)
                .ThenByDescending(p => p.PublishDate)
                .Skip(dto.PageSize * (dto.PageNumber - 1))
                .Take(dto.PageSize)
                .Select(p =>
                    p.PostType == PostType.Announcement
                        ? new
                        {
                            p.Id,
                            p.PostTitle,
                            p.Description,
                            PostPictures = p.PostPictureUrl,
                            p.CreationDate,
                            p.PublishDate,
                            p.TenantId,
                            p.CreatedBy,
                            PostType = PostType.Announcement,
                            Priority = p.Priority.ToString(),
                            p.IsPinned,
                            p.IsDraft,
                        }
                        : p.PostType == PostType.NewsFeed
                            ? new
                            {
                                p.Id,
                                p.PostTitle,
                                p.Description,
                                PostPictures = p.PostPictureUrl,
                                p.CreationDate,
                                p.PublishDate,
                                p.TenantId,
                                p.CreatedBy,
                                PostType = PostType.NewsFeed,
                                p.IsDraft,
                                PostTags = p.PostTags.Select(pt => new { pt.Id, pt.Tag }).ToList(),
                                p.HasComments,
                                p.HasLikes,
                                p.HasShares,
                                NumberOfLikes = p.HasLikes ? p.Likes.Count : (int?)null,
                                NumberOfComments = p.HasComments ? p.Comments.Count : (int?)null,
                                NumberOfShares = p.HasShares ? p.Shares.Count : (int?)null
                            }
                            : p.PostType == PostType.EventPoll && p.Poll != null
                                ? new
                                {
                                    p.Id,
                                    p.PostTitle,

                                    p.CreationDate,
                                    p.PublishDate,
                                    p.TenantId,
                                    p.CreatedBy,
                                    PostType = PostType.EventPoll,
                                    p.IsDraft,
                                    HasMultiselect = p.Poll.HasMultiSelect,
                                    p.Poll.Question,
                                    PollId = p.Poll.Id,
                                    Choices = p.Poll.Choices.Select(c => new
                                    {
                                        c.Id,
                                        c.Text,
                                        Votes = c.Poll.Choices.Sum(choice => choice.Vote) > 0
                                            ? (int)Math.Round((double)c.Vote / c.Poll.Choices.Sum(choice => choice.Vote) * 100)
                                            : 0
                                    }).ToList()
                                }
                                : (object)new
                                {
                                    p.Id,
                                    p.PostTitle,
                                    p.Description,
                                    PostPictures = p.PostPictureUrl,
                                    p.CreationDate,
                                    p.PublishDate,
                                    p.TenantId,
                                    p.CreatedBy,
                                    PostType = PostType.HighLights,
                                    p.IsDraft,
                                    p.HasComments,
                                    p.HasLikes,
                                    p.HasShares,
                                    NumberOfLikes = p.HasLikes ? p.Likes.Count : (int?)null,
                                    NumberOfComments = p.HasComments ? p.Comments.Count : (int?)null,
                                    NumberOfShares = p.HasShares ? p.Shares.Count : (int?)null
                                }
                )
                .ToListAsync();

            return posts;
        }


        public async Task<IReadOnlyList<object>> GetAllMobilePosts(GetAllMobilePostsDto dto, DateOnly date)
        {
            var posts = await _context.Posts
                .AsNoTracking()
                .Where(p => p.TenantId == dto.TenantId && !p.IsDraft && (!p.PublishDate.HasValue || p.PublishDate <= date) && (string.IsNullOrEmpty(dto.SearchTerm) || p.PostTitle.ToLower().Replace(" ", "").Contains(dto.SearchTerm) || p.Poll.Question.ToLower().Replace(" ", "").Contains(dto.SearchTerm)))
                .OrderByDescending(p => p.IsPinned)
                .ThenByDescending(p => p.PublishDate)
                .Skip(dto.PageSize * (dto.PageNumber - 1))
                .Take(dto.PageSize)
                .Select(p =>
                    p.PostType == PostType.Announcement
                        ? new
                        {
                            p.Id,
                            p.PostTitle,
                            p.Description,
                            PostPictures = p.PostPictureUrl,
                            p.CreationDate,
                            p.PublishDate,
                            p.TenantId,
                            p.CreatedBy,
                            PostType = PostType.Announcement,
                            Priority = p.Priority.ToString(),
                            p.IsPinned,
                            p.IsDraft,
                        }
                        : p.PostType == PostType.NewsFeed
                            ? new
                            {
                                p.Id,
                                p.PostTitle,
                                p.Description,
                                PostPictures = p.PostPictureUrl,
                                p.CreationDate,
                                p.PublishDate,
                                p.TenantId,
                                p.CreatedBy,
                                PostType = PostType.NewsFeed,
                                p.IsDraft,
                                PostTags = p.PostTags.Select(pt => new { pt.Id, pt.Tag }).ToList(),
                                p.HasComments,
                                p.HasLikes,
                                p.HasShares,
                                NumberOfLikes = p.HasLikes ? p.Likes.Count : (int?)null,
                                NumberOfComments = p.HasComments ? p.Comments.Count : (int?)null,
                                NumberOfShares = p.HasShares ? p.Shares.Count : (int?)null,
                                IsLiked = p.HasLikes ? p.Likes.Any(l => l.CreatedBy == dto.UserId) : (bool?)null,
                            }
                            : p.PostType == PostType.EventPoll && p.Poll != null
                                ? new
                                {
                                    p.Id,
                                    p.PostTitle,

                                    p.CreationDate,
                                    p.PublishDate,
                                    p.TenantId,
                                    p.CreatedBy,
                                    PostType = PostType.EventPoll,
                                    p.IsDraft,
                                    HasMultiselect = p.Poll.HasMultiSelect,
                                    p.Poll.Question,
                                    PollId = p.Poll.Id,
                                    Choices = p.Poll.Choices.Select(c => new
                                    {
                                        c.Id,
                                        c.Text,
                                        IsVoted = c.UserChoices.ToList().Any(c => c.CreatedBy == dto.UserId),
                                        Votes = c.Poll.Choices.Sum(choice => choice.Vote) > 0
                                            ? (int)Math.Round((double)c.Vote / c.Poll.Choices.Sum(choice => choice.Vote) * 100)
                                            : 0
                                    }).ToList()
                                }
                                : (object)new
                                {
                                    p.Id,
                                    p.PostTitle,
                                    p.Description,
                                    PostPictures = p.PostPictureUrl,
                                    p.CreationDate,
                                    p.PublishDate,
                                    p.TenantId,
                                    p.CreatedBy,
                                    PostType = PostType.HighLights,
                                    p.IsDraft,
                                    p.HasComments,
                                    p.HasLikes,
                                    p.HasShares,
                                    NumberOfLikes = p.HasLikes ? p.Likes.Count : (int?)null,
                                    NumberOfComments = p.HasComments ? p.Comments.Count : (int?)null,
                                    NumberOfShares = p.HasShares ? p.Shares.Count : (int?)null,
                                    IsLiked = p.HasLikes ? p.Likes.Any(l => l.CreatedBy == dto.UserId) : (bool?)null
                                }
                )
                .ToListAsync();

            return posts;
        }




    }
}
