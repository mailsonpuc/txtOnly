using Microsoft.EntityFrameworkCore;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;
using txtOnly.Infrastructure.Context;

namespace txtOnly.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;

    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<Comment>> GetCommentsPagedAsync(Guid postId, int pageNumber, int pageSize)
    {
        ValidatePagination(pageNumber, pageSize);
        var query = _context.Comments.AsNoTracking().Where(comment => comment.PostId == postId)
            .OrderBy(comment => comment.CreatedAt);
        return await PagedList<Comment>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task<Comment?> GetByIdAsync(Guid? id)
    {
        if (!id.HasValue || id.Value == Guid.Empty)
            return null;
        return await _context.Comments.AsNoTracking().FirstOrDefaultAsync(comment => comment.CommentId == id.Value);
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        await _context.Comments.AddAsync(comment);
        return comment;
    }

    public Task<Comment> UpdateAsync(Comment comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        _context.Comments.Update(comment);
        return Task.FromResult(comment);
    }

    public Task<Comment> RemoveAsync(Comment comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        _context.Comments.Remove(comment);
        return Task.FromResult(comment);
    }

    private static void ValidatePagination(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "A página deve ser maior que zero.");
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "O tamanho da página deve ser maior que zero.");
    }
}