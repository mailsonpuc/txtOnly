using txtOnly.Application.DTOs;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Interfaces;

public interface ICommentService
{
    Task<PagedList<CommentDTO>> GetCommentsPagedAsync(Guid postId, int pageNumber, int pageSize);
    Task<CommentDTO?> GetByIdAsync(Guid? id);
    Task<CommentDTO> CreateAsync(CommentCreateDTO comment);
    Task<CommentDTO> UpdateAsync(CommentDTO comment);
    Task<CommentDTO> RemoveAsync(CommentDTO comment);
}