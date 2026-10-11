using Microsoft.EntityFrameworkCore;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;
using txtOnly.Infrastructure.Context;

namespace txtOnly.Infrastructure.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly AppDbContext _context;

    public LikeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<Like>> GetLikesPagedAsync(Guid postId, int pageNumber, int pageSize)
    {
        var query = _context.Likes.AsNoTracking().Where(like => like.PostId == postId)
            .OrderByDescending(like => like.CreatedAt);
        return await PagedList<Like>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task<Like?> GetByUserAndPostAsync(Guid userId, Guid postId) =>
        await _context.Likes.AsNoTracking().FirstOrDefaultAsync(like => like.UserId == userId && like.PostId == postId);

    public async Task<Like> CreateAsync(Like like)
    {
        ArgumentNullException.ThrowIfNull(like);
        await _context.Likes.AddAsync(like);
        return like;
    }

    public Task<Like> RemoveAsync(Like like)
    {
        ArgumentNullException.ThrowIfNull(like);
        _context.Likes.Remove(like);
        return Task.FromResult(like);
    }

}