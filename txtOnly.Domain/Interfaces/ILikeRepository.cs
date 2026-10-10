using txtOnly.Domain.Entities;
using txtOnly.Domain.Pagination;

namespace txtOnly.Domain.Interfaces;

public interface ILikeRepository
{
    Task<PagedList<Like>> GetLikesPagedAsync(Guid postId, int pageNumber, int pageSize);
    Task<Like?> GetByUserAndPostAsync(Guid userId, Guid postId);
    Task<Like> CreateAsync(Like like);
    Task<Like> RemoveAsync(Like like);
}