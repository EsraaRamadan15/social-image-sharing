using Identity.Application.Abstractions.Repositories;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedInfrastructure.Persistence;

namespace Identity.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<User>()
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<User>()
                .FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<User>()
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<User>().AddAsync(user, cancellationToken);
        }
    }
}
