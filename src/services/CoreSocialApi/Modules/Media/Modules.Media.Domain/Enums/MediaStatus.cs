namespace MediaService.Domain.Enums
{

    public enum MediaStatus
    {
        Pending = 1,
        Uploading = 2,
        Uploaded = 3,
        Processing = 4,
        Ready = 5,
        Failed = 6,
        Deleted = 7
    }
}
