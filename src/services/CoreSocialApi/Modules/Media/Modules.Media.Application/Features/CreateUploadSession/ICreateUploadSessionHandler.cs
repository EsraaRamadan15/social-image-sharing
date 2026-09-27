using Application.Common;

namespace MediaService.Application.Features.CreateUploadSession
{
    public interface ICreateUploadSessionHandler
    {
        Task<Result<CreateUploadSessionResponse>> HandleAsync(
            CreateUploadSessionRequest request,
            CancellationToken cancellationToken = default);
    }
}
