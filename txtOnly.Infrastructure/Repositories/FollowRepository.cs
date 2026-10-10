using Microsoft.EntityFrameworkCore;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;
using txtOnly.Infrastructure.Context;

namespace txtOnly.Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly AppDbContext _context;

    public FollowRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<Follow>> GetFollowersPagedAsync(Guid userId, int pageNumber, int pageSize)
    {
        ValidatePagination(pageNumber, pageSize);
        var query = _context.Follows.AsNoTracking().Where(follow => follow.FollowingId == userId)
            .OrderByDescending(follow => follow.CreatedAt);
        return await PagedList<Follow>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task<PagedList<Follow>> GetFollowingPagedAsync(Guid userId, int pageNumber, int pageSize)
    {
        ValidatePagination(pageNumber, pageSize);
        var query = _context.Follows.AsNoTracking().Where(follow => follow.FollowerId == userId)
            .OrderByDescending(follow => follow.CreatedAt);
        return await PagedList<Follow>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task<Follow?> GetByUsersAsync(Guid followerId, Guid followingId) =>
        await _context.Follows.AsNoTracking().FirstOrDefaultAsync(
            follow => follow.FollowerId == followerId && follow.FollowingId == followingId);

    public async Task<Follow> CreateAsync(Follow follow)
    {
        ArgumentNullException.ThrowIfNull(follow);
        await _context.Follows.AddAsync(follow);
        return follow;
    }

    public Task<Follow> RemoveAsync(Follow follow)
    {
        ArgumentNullException.ThrowIfNull(follow);
        _context.Follows.Remove(follow);
        return Task.FromResult(follow);
    }

    private static void ValidatePagination(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "A página deve ser maior que zero.");
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "O tamanho da página deve ser maior que zero.");
    }
}