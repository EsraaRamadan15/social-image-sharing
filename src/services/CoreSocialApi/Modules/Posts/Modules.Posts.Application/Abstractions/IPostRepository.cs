using Posts.Domain.Entities;

namespace Posts.Application.Abstractions
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(Post post, CancellationToken cancellationToken = default);

        Task<List<Post>> GetUserPostsAsync(
            Guid userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);



        void Remove(Post post);
    }
}
