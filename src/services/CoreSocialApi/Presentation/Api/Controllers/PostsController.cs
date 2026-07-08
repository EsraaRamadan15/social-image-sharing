using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Application.Features.CreatePost;
using Posts.Application.Features.DeletePost;
using Posts.Application.Features.GetPost;
using Posts.Application.Features.GetUserPosts;

namespace Api.Controllers
{


    [ApiController]
    [Route("api/posts")]
    public sealed class PostsController : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(
            CreatePostRequest request,
            [FromServices] ICreatePostHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(request, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{postId:guid}")]
        public async Task<IActionResult> GetPost(
            Guid postId,
            [FromServices] IGetPostHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(postId, cancellationToken);

            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserPosts(
            Guid userId,
            [FromServices] IGetUserPostsHandler handler,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await handler.HandleAsync(userId, page, pageSize, cancellationToken);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpDelete("{postId:guid}")]
        public async Task<IActionResult> DeletePost(
            Guid postId,
            [FromServices] IDeletePostHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(postId, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}
