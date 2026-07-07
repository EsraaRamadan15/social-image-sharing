using Application.Common;

namespace MediaService.Application.Features.DeleteMedia
{
    public interface IDeleteMediaHandler
    {
        Task<Result> HandleAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default);
    }
}
