using txtOnly.Application.DTOs;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Interfaces;

public interface IFollowService
{
    Task<PagedList<FollowDTO>> GetFollowersPagedAsync(Guid userId, int pageNumber, int pageSize);
    Task<PagedList<FollowDTO>> GetFollowingPagedAsync(Guid userId, int pageNumber, int pageSize);
    Task<FollowDTO> CreateAsync(FollowCreateDTO follow);
    Task<FollowDTO> RemoveAsync(FollowDTO follow);
}