using MediaService.Domain.Entities;

namespace MediaService.Application.Abstractions
{
    public interface IMediaAssetRepository
    {
        Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default);
    }
}
