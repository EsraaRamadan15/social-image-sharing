namespace MediaService.Contracts.Posts.Models
{
    public enum MediaPostAccessFailure
    {
        NotFound = 1,
        NotOwnedByUser = 2,
        NotReady = 3,
        UnsupportedMediaType = 4
    }
}
