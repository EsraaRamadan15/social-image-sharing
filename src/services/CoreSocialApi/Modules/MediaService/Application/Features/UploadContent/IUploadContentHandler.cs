using Application.Common;

namespace MediaService.Application.Features.UploadContent
{
    public interface IUploadContentHandler
    {
        Task<Result<UploadContentResponse>> HandleAsync(
            UploadContentRequest request,
            CancellationToken cancellationToken = default);
    }
}
