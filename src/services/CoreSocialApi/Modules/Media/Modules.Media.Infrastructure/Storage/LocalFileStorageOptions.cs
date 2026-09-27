namespace MediaService.Infrastructure.Storage
{
    public sealed class LocalFileStorageOptions
    {
        public const string SectionName = "LocalStorage";

        public string RootPath { get; set; } = "Uploads";

        public string PublicBaseUrl { get; set; } = "/uploads";
    }
}
