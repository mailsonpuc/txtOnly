using txtOnly.Application.DTOs;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Interfaces;

public interface IPostService
{
    Task<List<PostDTO>> GetPostsAsync();
    Task<PagedList<PostDTO>> GetPostsPagedAsync(int pageNumber, int pageSize);
    Task<PagedList<PostDTO>> GetPostsByUserPagedAsync(Guid userId, int pageNumber, int pageSize);
    Task<PostDTO?> GetByIdAsync(Guid? id);
    Task<PostDTO> CreateAsync(PostCreateDTO post);
    Task<PostDTO> UpdateAsync(PostDTO post);
    Task<PostDTO> RemoveAsync(PostDTO post);
    Task<PostDTO> RestoreAsync(Guid id);
}