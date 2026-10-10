using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.Application.Mappings;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UserService(IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<PagedList<UserDTO>> GetUsersPagedAsync(int pageNumber, int pageSize)
    {
        var users = await _userRepository.GetUsersPagedAsync(pageNumber, pageSize);
        var userDtos = users.Select(user => user.ToUserDTO()).ToList();
        return new PagedList<UserDTO>(userDtos, users.TotalCount, users.CurrentPage, users.PageSize);
    }

    public async Task<UserDTO?> GetByIdAsync(Guid? id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user?.ToUserDTO();
    }

    public async Task<UserDTO> CreateAsync(UserCreateDTO user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var createdUser = await _userRepository.CreateAsync(user.ToUser());
        await _unitOfWork.CommitAsync();
        return createdUser.ToUserDTO();
    }

    public async Task<UserDTO> UpdateAsync(UserDTO user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var userEntity = await GetEntityByIdAsync(user.UserId);
        user.ApplyTo(userEntity);
        var updatedUser = await _userRepository.UpdateAsync(userEntity);
        await _unitOfWork.CommitAsync();
        return updatedUser.ToUserDTO();
    }

    private async Task<User> GetEntityByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O ID do usuário é obrigatório.", nameof(id));
        return await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");
    }
}