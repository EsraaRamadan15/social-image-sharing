using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Posts.Application;
using Posts.Application.Abstractions;
using Posts.Application.Features.CreatePost;
using Posts.Application.Features.DeletePost;
using Posts.Application.Features.GetPost;
using Posts.Application.Features.GetUserPosts;
using Posts.Infrastructure.Persistence.Repositories;
using SharedInfrastructure.Persistence;

namespace Posts.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPostsInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddModelConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddPostsApplication();
            services.AddScoped<IPostRepository, PostRepository>();

            services.AddScoped<ICreatePostHandler, CreatePostHandler>();
            services.AddScoped<IGetPostHandler, GetPostHandler>();
            services.AddScoped<IGetUserPostsHandler, GetUserPostsHandler>();
            services.AddScoped<IDeletePostHandler, DeletePostHandler>();



            return services;
        }
    }
}
