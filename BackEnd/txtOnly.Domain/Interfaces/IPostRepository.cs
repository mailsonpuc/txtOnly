using txtOnly.Domain.Entities;
using txtOnly.Domain.Pagination;

namespace txtOnly.Domain.Interfaces;

public interface IPostRepository
{
    Task<List<Post>> GetPostsAsync();
    Task<PagedList<Post>> GetPostsPagedAsync(int pageNumber, int pageSize);
    Task<PagedList<Post>> GetPostsByUserPagedAsync(Guid userId, int pageNumber, int pageSize);
    Task<Post?> GetByIdAsync(Guid? id);
    Task<Post> CreateAsync(Post post);
    Task<Post> UpdateAsync(Post post);
}