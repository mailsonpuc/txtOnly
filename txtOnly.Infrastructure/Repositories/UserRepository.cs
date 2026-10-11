using Microsoft.EntityFrameworkCore;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;
using txtOnly.Infrastructure.Context;

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

    public async Task<User?> GetByIdAsync(Guid? id)
    {
        if (!id.HasValue || id.Value == Guid.Empty)
            return null;
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.UserId == id.Value);
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