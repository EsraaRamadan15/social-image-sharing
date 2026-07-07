using MediaService.Domain.Enums;

namespace MediaService.Application.Features.GetMedia
{
    public sealed class GetMediaResponse
    {
        public Guid MediaId { get; init; }
        public string? OriginalFileName { get; init; }
        public string? PublicUrl { get; init; }
        public string? ContentType { get; init; }
        public long? FileSizeInBytes { get; init; }
        public MediaType MediaType { get; init; }
        public MediaStatus Status { get; init; }
        public DateTimeOffset CreatedAtUtc { get; init; }
    }
}
