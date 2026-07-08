using MediaService.Application.Features.CreateUploadSession;
using MediaService.Application.Features.DeleteMedia;
using MediaService.Application.Features.GetMedia;
using MediaService.Application.Features.UploadContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/media")]
    public sealed class MediaController : ControllerBase
    {
        [Authorize]
        [HttpPost("upload-session")]
        public async Task<IActionResult> CreateUploadSession(
            CreateUploadSessionRequest request,
            [FromServices] ICreateUploadSessionHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(request, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPost("{mediaId:guid}/content")]
        public async Task<IActionResult> UploadContent(
            Guid mediaId,
            IFormFile file,
            [FromServices] IUploadContentHandler handler,
            CancellationToken cancellationToken)
        {
            if (file.Length == 0)
                return BadRequest("File is required.");

            await using var stream = file.OpenReadStream();

            var request = new UploadContentRequest
            {
                MediaId = mediaId,
                FileStream = stream,
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                FileSizeInBytes = file.Length
            };

            var result = await handler.HandleAsync(request, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpGet("{mediaId:guid}")]
        public async Task<IActionResult> GetMedia(
            Guid mediaId,
            [FromServices] IGetMediaHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(mediaId, cancellationToken);

            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpDelete("{mediaId:guid}")]
        public async Task<IActionResult> DeleteMedia(
            Guid mediaId,
            [FromServices] IDeleteMediaHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(mediaId, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}

