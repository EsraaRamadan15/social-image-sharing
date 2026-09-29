using Application.Common;
using MediaService.Contracts.Posts;
using Posts.Application.Abstractions;
using Posts.Application.Features.GetPost.Models;

namespace Posts.Application.Features.GetUserPosts
{
    public sealed class GetUserPostsHandler : IGetUserPostsHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly IMediaPostQueryService _mediaQueryService;

        public GetUserPostsHandler(IPostRepository postRepository, IMediaPostQueryService mediaQueryService)
        {
            _postRepository = postRepository;
            _mediaQueryService = mediaQueryService;
        }

        public async Task<Result<GetUserPostsResponse>> HandleAsync(
            Guid userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 50);

            var posts = await _postRepository.GetUserPostsAsync(
                userId,
                page,
                pageSize,
                cancellationToken);
            var mediaIds = posts
                    .Select(x => x.MediaId)
                    .Distinct()
                    .ToArray();

            var mediaById = await _mediaQueryService.GetForPostsAsync(
                mediaIds,
                cancellationToken);
            var items = posts.Select(post =>
            {
                mediaById.TryGetValue(
                    post.MediaId,
                    out var media);

                return new GetPostResponse
                {
                    PostId = post.Id,
                    UserId = post.UserId,
                    Caption = post.Caption,
                    Visibility = post.Visibility,

                    Media = media is null
                        ? null
                        : new PostMediaResponse
                        {
                            MediaId = media.MediaId,
                            Url = media.Url,
                            ContentType = media.ContentType
                        },

                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    CreatedAtUtc = post.CreatedAtUtc
                };
            }).ToList();


            return Result<GetUserPostsResponse>.Success(new GetUserPostsResponse
            {
                Items = items
            });
        }
    }
}
