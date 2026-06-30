namespace MediaService.Application.Abstractions
{
    public interface IFileStorage
    {
        Task<StoredFileResult> SaveAsync(
            Stream fileStream,
            string originalFileName,
            string contentType,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);
    }

    public sealed record StoredFileResult(
        string StorageFileName,
        string StoragePath,
        string PublicUrl,
        long SizeInBytes);
}
