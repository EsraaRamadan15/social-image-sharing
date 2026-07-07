using Application.Common;

namespace MediaService.Application.Features.GetMedia
{
    public interface IGetMediaHandler
    {
        Task<Result<GetMediaResponse>> HandleAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default);
    }
}
