namespace MediaService.Contracts.Posts.Models
{
    public sealed record MediaPostAccessResult(
    bool IsAllowed,
    MediaPostAccessFailure? Failure);
}
