using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Extensions;
using System.Net.Http.Headers;
using MyEntityMembershipService.Repositories.Entity.CommentRepo;
using MyEntityMembershipService.Services.Entity.PostService;
using MyEntityMembershipService.DTOs.Entity.MobileDtos;

using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.Services.Entity.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPostService _postService;

        public CommentService(ICommentRepository commentRepository, IUnitOfWork unitOfWork, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IPostService postService)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _postService = postService;
        }

        public async Task<Response<object>> AddComment(AddCommentDto dto)
        {
            try
            {

                var post = await _postService.GetPostByIdAsyncAndIsNotDraft(dto.PostId);

                var postTypeValidation = ValidateOnPostType(post);

                if (postTypeValidation.Length > 0)
                    return new ResponseHandler().BadRequest<object>(postTypeValidation);


                var httpClient = new HttpClient();

                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

                token = token.Replace("Bearer ", "");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var link = $"{_configuration["ApiSettings:Gateway"]}{_configuration["MySelfOldUrl:GetUsers"]}";

                var userAsList = new List<string>() { dto.UserId };

                var userData = await httpClient.PostAsJson(link, userAsList);


                if (userData[0].id == null)
                    return new ResponseHandler().NotFound<object>("Invalid userId");

                if (dto.ParentCommentId.HasValue)
                {
                    var comment = await _commentRepository.GetByIdAsync(dto.ParentCommentId.Value);

                    if (comment is null)
                        return new ResponseHandler().NotFound<object>("Invalid parent commentId");

                    var commentToAdd = new Comment { CreatedBy = dto.UserId, PostId = comment.PostId, Text = dto.Text, ParentCommentId = dto.ParentCommentId.Value };
                    await _commentRepository.Add(commentToAdd);

                    await _commentRepository.SaveAsync();

                    var totalComments = await _unitOfWork.Repository<Comment>().GetCountByCriteriaAsync(l => l.PostId == dto.PostId);

                    var dtoToReturn = new { commentToAdd.Id, UserName = (string)userData[0].userName, CreatedBy = dto.UserId, dto.Text, dto.PostId, dto.ParentCommentId, CreatedAt = commentToAdd.CreationDate, TotalComments = totalComments };

                    return new ResponseHandler().Created<object>(dtoToReturn);
                }
                else
                {
                    var commentToAdd = new Comment { CreatedBy = dto.UserId, PostId = dto.PostId, Text = dto.Text };

                    await _commentRepository.Add(commentToAdd);

                    await _commentRepository.SaveAsync();

                    var totalComments = await _unitOfWork.Repository<Comment>().GetCountByCriteriaAsync(l => l.PostId == dto.PostId);


                    var dtoToReturn = new { commentToAdd.Id, UserName = (string)userData[0].userName, CreatedBy = dto.UserId, dto.Text, dto.PostId, dto.ParentCommentId, CreatedAt = commentToAdd.CreationDate, TotalComments = totalComments };

                    return new ResponseHandler().Created<object>(dtoToReturn);
                }

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<object>("Something went wrong");

            }
        }

        public async Task<Response<object>> EditComment(int commentId, string userId, string text)
        {
            try
            {


                var comment = await _commentRepository.GetByIdAsync(commentId);

                if (comment is null)
                    return new ResponseHandler().NotFound<object>("Invalid commentId");

                if (comment.CreatedBy != userId)
                    return new ResponseHandler().BadRequest<object>("Huh sneaking around? you are not the author of this comment");

                var httpClient = new HttpClient();

                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

                token = token.Replace("Bearer ", "");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var link = $"{_configuration["ApiSettings:Gateway"]}{_configuration["MySelfOldUrl:GetUsers"]}";

                var userAsList = new List<string>() { userId };

                var userData = await httpClient.PostAsJson(link, userAsList);

                if (userData[0].id == null)
                    return new ResponseHandler().NotFound<object>("Invalid userId");


                comment.Text = text;

                await _commentRepository.SaveAsync();

                var totalComments = await _unitOfWork.Repository<Comment>().GetCountByCriteriaAsync(l => l.PostId == comment.PostId);

                var dtoToReturn = new { comment.Id, UserName = (string)userData[0].userName, CreatedBy = userId, Text = text, comment.PostId, comment.ParentCommentId, CreatedAt = comment.CreationDate, ModifiedAt = comment.ModificationDate, TotalComments = totalComments };



                return new ResponseHandler().EditSuccess<object>(dtoToReturn);


            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<object>("Something went wrong");

            }
        }

        public async Task<Response<int>> DeleteComment(int commentId, string userId)
        {
            try
            {


                var comment = await _commentRepository.GetByIdAsync(commentId);

                if (comment is null)
                    return new ResponseHandler().NotFound<int>("Invalid commentId");

                if (comment.CreatedBy != userId)
                    return new ResponseHandler().BadRequest<int>("Huh sneaking around? you are not the author of this comment");


                var comments = await _commentRepository.GetCommentAndRepliesByIdAsync(commentId);

                _commentRepository.SoftDeleteRange(comments.ToList());

                await _commentRepository.SaveAsync();

                var totalComments = await _unitOfWork.Repository<Comment>().GetCountByCriteriaAsync(l => l.PostId == comment.PostId);

                return new ResponseHandler().Success(totalComments);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<int>("Something went wrong");

            }
        }

        public async Task<int> GetCountOfCommentsOfPost(int postId)

             => await _commentRepository.GetCountOfCommentsOfPostAsync(postId);

        //public async Task<Response<string>> AddReplyOnComment(AddReplyDto dto)
        //{
        //    try
        //    {

        //        var comment = await _commentRepository.GetByIdAsync(dto.CommentId);

        //        if (comment is null)
        //            return new ResponseHandler().NotFound<string>("Invalid commentId");

        //        var httpClient = new HttpClient();

        //        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        //        token = token.Replace("Bearer ", "");

        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        var link = $"{_configuration["MySelfOldUrl:GetUsers"]}";

        //        var userAsList = new List<string>() { dto.UserId };

        //        var userData = await httpClient.PostAsJson(link, userAsList);

        //        if (userData[0].id == null)
        //            return new ResponseHandler().NotFound<string>("Invalid userId");

        //        await _commentRepository.Add(new Comment { CreatedBy = dto.UserId, PostId = comment.PostId, Text = dto.Text, ParentCommentId = dto.CommentId });

        //        await _commentRepository.SaveAsync();

        //        return new ResponseHandler().Created<string>("reply added successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseHandler().BadRequest<string>("Something went wrong");

        //    }
        //}

        public async Task<Response<List<CommentDto>>> GetCommentsWithReplies(int postId)
        {
            try
            {

                var comments = await _commentRepository.GetCommentsWithReplies(postId);

                var users = new List<string>();

                for (int i = 0; i < comments.Count; i++)
                {
                    users.Add(comments[i].CreatedBy);

                    var usersOfReplies = GetUsersOfReplies(comments[i].Replies);

                    users.AddRange(usersOfReplies);
                }

                users = users.DistinctBy(u => u).ToList();

                var usersDataAsList = new List<dynamic>();

                if (users.Count > 0)
                {
                    var httpClient = new HttpClient();

                    var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

                    token = token.Replace("Bearer ", "");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var link = $"{_configuration["ApiSettings:Gateway"]}{_configuration["MySelfOldUrl:GetUsers"]}";

                    var userData = await httpClient.PostAsJson(link, users);

                    if (userData[0].id == null)
                        return new ResponseHandler().NotFound<List<CommentDto>>("Invalid userId");

                    usersDataAsList = ((IEnumerable<dynamic>)userData).ToList();
                }



                for (int i = 0; i < comments.Count; i++)
                {

                    var user = usersDataAsList.FirstOrDefault(u => (string)u.id == comments[i].CreatedBy);

                    if (user is not null)
                        comments[i].UserName = (string)user.userName;

                    PopulateUsersOfReplies(comments[i].Replies, usersDataAsList);
                }

                return new ResponseHandler().Success(comments);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<CommentDto>>("Something went wrong");

            }
        }

        private List<string> GetUsersOfReplies(List<CommentDto> dto)
        {
            var users = new List<string>();

            for (int i = 0; i < dto.Count; i++)
            {
                users.Add(dto[i].CreatedBy);

                var usersOfReplies = GetUsersOfReplies(dto[i].Replies);

                users.AddRange(usersOfReplies);
            }

            return users;

        }

        private void PopulateUsersOfReplies(List<CommentDto> dto, List<dynamic> users)
        {

            for (int i = 0; i < dto.Count; i++)
            {
                var user = users.FirstOrDefault(u => (string)u.id == dto[i].CreatedBy);

                if (user is not null)
                    dto[i].UserName = (string)user.userName;

                PopulateUsersOfReplies(dto[i].Replies, users);
            }

        }

        private string ValidateOnPostType(Post post)
        {

            if (post is null)
                return "Invalid postId";

            if (post.PostType == PostType.Announcement)
                return "Announcements don't have comments";

            if (post.PostType == PostType.EventPoll)
                return "Event Polls don't have comments";

            if (post.PostType == PostType.EventUpdate)
                return "Event Updates don't have comments";

            if (post.PostType == PostType.NewsFeed && post.HasComments == false)
                return "comments are disabled for this post";

            if (post.PostType == PostType.HighLights && post.HasComments == false)
                return "comments are disabled for this post";

            return "";
        }



        private async Task<string> ValidateOnUser(string userId)
        {
            var httpClient = new HttpClient();

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            token = token.Replace("Bearer ", "");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var link = $"{_configuration["ApiSettings:Gateway"]}{_configuration["MySelfOldUrl:GetUsers"]}";

            var userAsList = new List<string>() { userId };

            var userData = await httpClient.PostAsJson(link, userAsList);

            if (userData[0].id == null)
                return "Invalid userId";

            return "";

        }
    }
}
