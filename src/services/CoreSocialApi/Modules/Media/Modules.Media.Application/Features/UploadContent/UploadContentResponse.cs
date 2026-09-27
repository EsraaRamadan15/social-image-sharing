using MediaService.Domain.Enums;

namespace MediaService.Application.Features.UploadContent
{
    public sealed class UploadContentResponse
    {
        public Guid MediaId { get; init; }
        public string PublicUrl { get; init; } = string.Empty;
        public MediaStatus Status { get; init; }
    }
}
