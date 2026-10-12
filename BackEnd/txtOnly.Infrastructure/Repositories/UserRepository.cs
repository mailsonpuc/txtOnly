using Microsoft.EntityFrameworkCore;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;
using txtOnly.Infrastructure.Context;
using txtOnly.Infrastructure.Identity;

namespace txtOnly.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<User>> GetUsersPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Users.AsNoTracking().OrderByDescending(user => user.CreatedAt);
        return await PagedList<User>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task<PagedList<User>> GetUsersByNamePagedAsync(string userName, int pageNumber, int pageSize)
    {
        var normalizedName = userName.Trim().ToUpperInvariant();
        var query =
            from user in _context.Users.AsNoTracking()
            join identityUser in _context.Set<ApplicationUser>().AsNoTracking()
                on user.UserId equals identityUser.Id
            where identityUser.NormalizedUserName != null && identityUser.NormalizedUserName.Contains(normalizedName)
            orderby user.CreatedAt descending
            select user;

        return await PagedList<User>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task<User?> GetByIdAsync(Guid? id)
    {
        if (!id.HasValue || id.Value == Guid.Empty)
            return null;
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.UserId == id.Value);
    }

    public async Task<IReadOnlyDictionary<Guid, string>> GetUserNamesAsync(IEnumerable<Guid> userIds)
    {
        var ids = userIds.Distinct().ToArray();
        if (ids.Length == 0)
            return new Dictionary<Guid, string>();

        return await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(user => ids.Contains(user.Id))
            .ToDictionaryAsync(user => user.Id, user => user.UserName ?? string.Empty);
    }

    public async Task<User> CreateAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        await _context.Users.AddAsync(user);
        return user;
    }

    public Task<User> UpdateAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        _context.Users.Update(user);
        return Task.FromResult(user);
    }

}