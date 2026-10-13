namespace MediaService.Contracts.Profiles.Models
{
    public sealed record MediaAvatarAccessResult(
        bool IsAllowed,
        MediaAvatarAccessFailure? Failure);

    public enum MediaAvatarAccessFailure
    {
        NotFound = 1,
        NotOwnedByUser = 2,
        NotReady = 3,
        UnsupportedMediaType = 4
    }

}
