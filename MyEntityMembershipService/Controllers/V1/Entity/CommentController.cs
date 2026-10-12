using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.MobileDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Services.Entity.CommentService;

namespace MyEntityMembershipService.Controllers.V1.Entity
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;


        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;

        }

        [HttpPost("AddComment")]
        public async Task<IResult> AddComment(AddCommentDto dto)
           => (await _commentService.AddComment(dto)).GetResponse();

        [HttpPut("EditComment")]
        public async Task<IResult> EditComment([FromQuery] int commentId, string userId, string text)
           => (await _commentService.EditComment(commentId, userId, text)).GetResponse();

        [HttpPut("DeleteComment")]
        public async Task<IResult> DeleteComment([FromQuery] int commentId, string userId)
          => (await _commentService.DeleteComment(commentId, userId)).GetResponse();

        //[HttpPost("AddReplyOnComment")]
        //public async Task<IResult> AddReplyOnComment(AddReplyDto dto)
        //   => (await _commentService.AddReplyOnComment(dto)).GetResponse();

        [HttpGet("GetCommentsOfPosts")]
        public async Task<Response<List<CommentDto>>> GetCommentsOfPosts([FromQuery] int postId)
           => (await _commentService.GetCommentsWithReplies(postId));
    }
}
