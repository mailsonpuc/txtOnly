using txtOnly.Domain.Entities;
using txtOnly.Domain.Pagination;

namespace txtOnly.Domain.Interfaces;

public interface IFollowRepository
{
    Task<PagedList<Follow>> GetFollowersPagedAsync(Guid userId, int pageNumber, int pageSize);
    Task<PagedList<Follow>> GetFollowingPagedAsync(Guid userId, int pageNumber, int pageSize);
    Task<Follow?> GetByUsersAsync(Guid followerId, Guid followingId);
    Task<Follow> CreateAsync(Follow follow);
    Task<Follow> RemoveAsync(Follow follow);
}