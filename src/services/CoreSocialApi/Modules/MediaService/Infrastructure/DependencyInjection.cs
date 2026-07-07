using MediaService.Application.Abstractions;
using MediaService.Application.Features.CreateUploadSession;
using MediaService.Application.Features.DeleteMedia;
using MediaService.Application.Features.GetMedia;
using MediaService.Application.Features.UploadContent;
using MediaService.Application.Options;
using MediaService.Infrastructure.Persistence.Repositories;
using MediaService.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedInfrastructure.Persistence;

namespace MediaService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMediaInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            string contentRootPath)
        {
            services.AddModelConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);

            services.Configure<MediaUploadOptions>(
                configuration.GetSection(MediaUploadOptions.SectionName));

            var localStorageOptions = configuration
                .GetSection(LocalFileStorageOptions.SectionName)
                .Get<LocalFileStorageOptions>() ?? new LocalFileStorageOptions();

            localStorageOptions.RootPath = ResolveRootPath(
                localStorageOptions.RootPath,
                contentRootPath);

            services.AddSingleton(localStorageOptions);

            services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();
            services.AddScoped<ILocalPathProvider, LocalPathProvider>();
            services.AddScoped<IFileStorage, LocalFileStorage>();

            services.AddScoped<ICreateUploadSessionHandler, CreateUploadSessionHandler>();
            services.AddScoped<IUploadContentHandler, UploadContentHandler>();
            services.AddScoped<IGetMediaHandler, GetMediaHandler>();
            services.AddScoped<IDeleteMediaHandler, DeleteMediaHandler>();

            return services;
        }

        private static string ResolveRootPath(string rootPath, string contentRootPath)
        {
            var configuredRootPath = string.IsNullOrWhiteSpace(rootPath)
                ? "Uploads"
                : rootPath;

            return Path.IsPathRooted(configuredRootPath)
                ? Path.GetFullPath(configuredRootPath)
                : Path.GetFullPath(Path.Combine(contentRootPath, configuredRootPath));
        }
    }
}
