using txtOnly.Application.DTOs;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Interfaces;

public interface IUserService
{
    Task<PagedList<UserDTO>> GetUsersPagedAsync(int pageNumber, int pageSize);
    Task<PagedList<UserDTO>> GetUsersByNamePagedAsync(string userName, int pageNumber, int pageSize);
    Task<UserDTO?> GetByIdAsync(Guid? id);
    Task<UserDTO> CreateAsync(UserCreateDTO user);
    Task<UserDTO> UpdateAsync(UserDTO user);
}