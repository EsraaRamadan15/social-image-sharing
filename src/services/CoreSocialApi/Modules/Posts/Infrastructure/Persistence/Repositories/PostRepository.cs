using Microsoft.EntityFrameworkCore;
using Posts.Application.Abstractions;
using Posts.Domain.Entities;
using SharedInfrastructure.Persistence;

namespace Posts.Infrastructure.Persistence.Repositories
{


    public sealed class PostRepository : IPostRepository
    {
        private readonly AppDbContext _dbContext;

        public PostRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Post?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Post>()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(
            Post post,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<Post>().AddAsync(post, cancellationToken);
        }

        public async Task<List<Post>> GetUserPostsAsync(
            Guid userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Post>()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public void Remove(Post post)
        {
            _dbContext.Set<Post>().Remove(post);
        }
    }
}
