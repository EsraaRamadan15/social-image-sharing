namespace MediaService.Application.Features.UploadContent
{
    public sealed class UploadContentRequest
    {
        public Guid MediaId { get; init; }
        public Stream FileStream { get; init; } = Stream.Null;
        public string OriginalFileName { get; init; } = string.Empty;
        public string ContentType { get; init; } = string.Empty;
        public long FileSizeInBytes { get; init; }
    }
}
