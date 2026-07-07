using MediaService.Application.Abstractions;
using MediaService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedInfrastructure.Persistence;

namespace MediaService.Infrastructure.Persistence.Repositories
{
    public sealed class MediaAssetRepository : IMediaAssetRepository
    {
        private readonly AppDbContext _dbContext;

        public MediaAssetRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MediaAsset?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<MediaAsset>()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(
            MediaAsset mediaAsset,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<MediaAsset>()
                .AddAsync(mediaAsset, cancellationToken);
        }
    }
}
