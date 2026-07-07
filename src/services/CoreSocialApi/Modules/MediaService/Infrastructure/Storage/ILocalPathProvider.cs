namespace MediaService.Infrastructure.Storage
{
    public interface ILocalPathProvider
    {
        string BuildStoragePath(string fileName);
    }
}
