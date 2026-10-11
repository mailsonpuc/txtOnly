using Microsoft.EntityFrameworkCore;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;
using txtOnly.Infrastructure.Context;

namespace txtOnly.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Post>> GetPostsAsync()
    {
        return await _context.Posts.AsNoTracking()
            .Where(post => !post.IsDeleted)
            .OrderByDescending(post => post.CreatedAt)
            .ToListAsync();
    }

    public async Task<PagedList<Post>> GetPostsPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Posts.AsNoTracking()
            .Where(post => !post.IsDeleted)
            .OrderByDescending(post => post.CreatedAt);
        return await PagedList<Post>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task<PagedList<Post>> GetPostsByUserPagedAsync(Guid userId, int pageNumber, int pageSize)
    {
        var query = _context.Posts.AsNoTracking()
            .Where(post => post.UserId == userId && !post.IsDeleted)
            .OrderByDescending(post => post.CreatedAt);
        return await PagedList<Post>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task<Post?> GetByIdAsync(Guid? id)
    {
        if (!id.HasValue || id.Value == Guid.Empty)
            return null;
        return await _context.Posts.AsNoTracking().FirstOrDefaultAsync(post => post.PostId == id.Value);
    }

    public async Task<Post> CreateAsync(Post post)
    {
        ArgumentNullException.ThrowIfNull(post);
        await _context.Posts.AddAsync(post);
        return post;
    }

    public Task<Post> UpdateAsync(Post post)
    {
        ArgumentNullException.ThrowIfNull(post);
        _context.Posts.Update(post);
        return Task.FromResult(post);
    }

}