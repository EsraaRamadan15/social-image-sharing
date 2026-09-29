namespace MediaService.Contracts.Posts.Models
{
    public sealed record MediaPostInfo(
    Guid MediaId,
    string Url,
    string ContentType);
}
