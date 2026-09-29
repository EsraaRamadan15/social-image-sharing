namespace Posts.Application.Features.GetPost.Models
{
    public sealed class PostMediaResponse
    {
        public Guid MediaId { get; init; }

        public string Url { get; init; } = string.Empty;

        public string ContentType { get; init; } = string.Empty;
    }
}
