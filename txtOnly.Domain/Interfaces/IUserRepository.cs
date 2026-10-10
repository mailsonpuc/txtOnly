using txtOnly.Domain.Entities;
using txtOnly.Domain.Pagination;

namespace txtOnly.Domain.Interfaces;

public interface IUserRepository
{
    Task<PagedList<User>> GetUsersPagedAsync(int pageNumber, int pageSize);
    Task<User?> GetByIdAsync(Guid? id);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
}