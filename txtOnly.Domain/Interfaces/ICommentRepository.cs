using txtOnly.Domain.Entities;
using txtOnly.Domain.Pagination;

namespace txtOnly.Domain.Interfaces;

public interface ICommentRepository
{
    Task<PagedList<Comment>> GetCommentsPagedAsync(Guid postId, int pageNumber, int pageSize);
    Task<Comment?> GetByIdAsync(Guid? id);
    Task<Comment> CreateAsync(Comment comment);
    Task<Comment> UpdateAsync(Comment comment);
    Task<Comment> RemoveAsync(Comment comment);
}