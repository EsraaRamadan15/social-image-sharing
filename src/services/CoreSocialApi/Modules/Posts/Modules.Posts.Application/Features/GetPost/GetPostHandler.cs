using Application.Common;
using MediaService.Contracts.Posts;
using Posts.Application.Abstractions;
using Posts.Application.Errors;
using Posts.Application.Features.GetPost.Models;

namespace Posts.Application.Features.GetPost
{


    public sealed class GetPostHandler : IGetPostHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly IMediaPostQueryService _mediaQueryService;


        public GetPostHandler(IPostRepository postRepository, IMediaPostQueryService mediaQueryService)
        {
            _postRepository = postRepository;
            _mediaQueryService = mediaQueryService;
        }

        public async Task<Result<GetPostResponse>> HandleAsync(
        Guid postId,
        CancellationToken cancellationToken = default)
        {
            var post = await _postRepository.GetByIdAsync(
                postId,
                cancellationToken);

            if (post is null)
            {
                return Result<GetPostResponse>.Failure(
                    PostErrors.NotFound);
            }

            var media = await _mediaQueryService.GetForPostAsync(
                post.MediaId,
                cancellationToken);

            var response = new GetPostResponse
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

            return Result<GetPostResponse>.Success(response);
        }
    }
}
