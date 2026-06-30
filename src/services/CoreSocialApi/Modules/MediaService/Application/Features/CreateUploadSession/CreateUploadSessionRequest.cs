using MediaService.Domain.Enums;

namespace MediaService.Application.Features.CreateUploadSession
{
    public sealed class CreateUploadSessionRequest
    {
        public MediaType MediaType { get; init; } = MediaType.Image;
    }
}
