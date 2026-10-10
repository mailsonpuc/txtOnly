using txtOnly.Application.DTOs;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Interfaces;

public interface ILikeService
{
    Task<PagedList<LikeDTO>> GetLikesPagedAsync(Guid postId, int pageNumber, int pageSize);
    Task<LikeDTO> CreateAsync(LikeCreateDTO like);
    Task<LikeDTO> RemoveAsync(LikeDTO like);
}