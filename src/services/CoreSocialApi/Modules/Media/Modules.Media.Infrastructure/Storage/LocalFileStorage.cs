using MediaService.Application.Abstractions;

namespace MediaService.Infrastructure.Storage
{
    public sealed class LocalFileStorage : IFileStorage
    {
        private readonly ILocalPathProvider _pathProvider;
        private readonly LocalFileStorageOptions _options;

        public LocalFileStorage(
            ILocalPathProvider pathProvider,
            LocalFileStorageOptions options)
        {
            _pathProvider = pathProvider;
            _options = options;
        }

        public async Task<StoredFileResult> SaveAsync(
            Stream fileStream,
            string originalFileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            var extension = Path.GetExtension(originalFileName);
            var storageFileName = $"{Guid.NewGuid():N}{extension}";

            var storagePath = _pathProvider.BuildStoragePath(storageFileName);

            var directory = Path.GetDirectoryName(storagePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var outputStream = File.Create(storagePath);
            await fileStream.CopyToAsync(outputStream, cancellationToken);

            var relativePath = Path.GetRelativePath(_options.RootPath, storagePath)
                .Replace(Path.DirectorySeparatorChar, '/')
                .Replace(Path.AltDirectorySeparatorChar, '/');

            var publicUrl = $"{NormalizePublicBaseUrl(_options.PublicBaseUrl)}/{relativePath}";

            var fileInfo = new FileInfo(storagePath);

            return new StoredFileResult(
                storageFileName,
                storagePath,
                publicUrl,
                fileInfo.Length);
        }

        public Task DeleteAsync(
            string storagePath,
            CancellationToken cancellationToken = default)
        {
            if (File.Exists(storagePath))
            {
                File.Delete(storagePath);
            }

            return Task.CompletedTask;
        }

        private static string NormalizePublicBaseUrl(string publicBaseUrl)
        {
            var trimmed = string.IsNullOrWhiteSpace(publicBaseUrl)
                ? "/uploads"
                : publicBaseUrl.Trim();

            if (Uri.TryCreate(trimmed, UriKind.Absolute, out _))
            {
                return trimmed.TrimEnd('/');
            }

            return $"/{trimmed.Trim('/')}";
        }
    }
}
