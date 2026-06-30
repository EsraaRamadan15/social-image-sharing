using MediaService.Domain.Enums;

namespace MediaService.Application.Features.CreateUploadSession
{
    public sealed class CreateUploadSessionResponse
    {
        public Guid MediaId { get; init; }
        public MediaStatus Status { get; init; }
    }
}
