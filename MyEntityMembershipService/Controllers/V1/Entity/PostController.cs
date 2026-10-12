using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.PostDtos;
using MyEntityMembershipService.DTOs.Entity.VoteDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.PollServices;
using MyEntityMembershipService.Services.Entity.PostService;

namespace MyEntityMembershipService.Controllers.V1.Entity
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IPollService _pollService;

        public PostController(IPostService postService, IPollService pollService)
        {
            _postService = postService;
            _pollService = pollService;
        }

        //[HttpPost("CreatePost")]
        //public async Task<IResult> CreatePost([FromForm] PostDto postDto)
        //    => (await _postService.CreatePost(postDto)).GetResponse();


        //[HttpGet("GetPostWithComments")]
        //public async Task<IResult> GetPostWithComments([FromQuery] int postId, string userId)

        // => (await _postService.GetPostWithComments(postId, userId)).GetResponse();


        //[HttpGet("GetPostsByTenantId")]
        //public async Task<IResult> GetPostsByTenantId([FromQuery] int tenantId, int pageNumber, int pageSize)

        //    => (await _postService.GetPostsByTenantIdWithPagination(tenantId, pageNumber, pageSize)).GetResponse();

        //[HttpGet("GetMobilePosts")]
        //public async Task<IResult> GetMobilePostsByTenantId([FromQuery] GetAllPostsDto dto)

        // => (await _postService.GetPostsForMobileByTenantIdWithPagination(dto)).GetResponse();


        //[HttpPost("BookMarkPost")]
        //public async Task<IResult> BookMarkPost([FromQuery] int postId, string userId)

        // => (await _postService.BookMarkPost(postId, userId)).GetResponse();

        //[HttpPost("UnBookMarkPost")]
        //public async Task<IResult> UnBookMarkPost([FromQuery] int postId, string userId)

        // => (await _postService.UnBookMarkPost(postId, userId)).GetResponse();

        [HttpPost("CreateAnnouncement")]
        public async Task<IResult> CreateAnnouncement([FromForm] AnnouncementDto dto)
           => (await _postService.CreateAnnouncementPost(dto)).GetResponse();

        [HttpPost("CreateNewsFeed")]
        public async Task<IResult> CreateNewsFeed([FromForm] NewsFeedDto dto)
           => (await _postService.CreateNewsFeedPost(dto)).GetResponse();

        [HttpPost("CreatePoll")]
        public async Task<IResult> CreatePoll([FromForm] PollDto dto)
          => (await _postService.CreatePollPost(dto)).GetResponse();

        [HttpPost("CreateHighlight")]
        public async Task<IResult> CreateHighlight([FromForm] HighlightDto dto)
          => (await _postService.CreateHighlightPost(dto)).GetResponse();


        [HttpGet("GetAllPosts")]
        public async Task<IResult> GetAllPosts([FromQuery] GetAllPostsWebDto dto)
          => (await _postService.GetAllPosts(dto)).GetResponse();

        [HttpGet("GetAllMobilePosts")]
        public async Task<IResult> GetAllMobilePosts([FromQuery] GetAllMobilePostsDto dto)
          => (await _postService.GetAllMobilePosts(dto)).GetResponse();

        [HttpPost("LikeToggle/{postId}/{userId}")]
        public async Task<IResult> LikeToggle(int postId, string userId)
          => (await _postService.LikeUnLikePost(postId, userId)).GetResponse();

        [HttpGet("GetPostsOfOtherEntities/{tenantId}")]
        public async Task<IResult> GetPostsOfOtherEntities(int tenantId)
            => (await _postService.GetPostsOfOtherEntities(tenantId)).GetResponse();


        [HttpGet("GetPostById/{postId}")]
        public async Task<IResult> GetPostById(int postId)
            => (await _postService.GetPostByIdAsync(postId)).GetResponse();

        [HttpPost("SharePost/{postId}/{userId}")]
        public async Task<IResult> SharePost(int postId, string userId)
            => (await _postService.SharePost(postId, userId)).GetResponse();


        [HttpPost("VoteOnAPoll")]
        public async Task<IResult> VoteOnAPoll(VoteDto pollDto)
           => (await _pollService.VoteOnAPoll(pollDto)).GetResponse();


    }
}
