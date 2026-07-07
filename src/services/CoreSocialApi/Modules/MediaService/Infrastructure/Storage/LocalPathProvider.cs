namespace MediaService.Infrastructure.Storage
{
    public sealed class LocalPathProvider
     : ILocalPathProvider
    {
        private readonly LocalFileStorageOptions _options;

        public LocalPathProvider(LocalFileStorageOptions options)
        {
            _options = options;
        }

        public string BuildStoragePath(string fileName)
        {
            var today = DateTime.UtcNow;

            return Path.Combine(
                _options.RootPath,
                today.Year.ToString(),
                today.Month.ToString("00"),
                fileName);
        }
    }

}