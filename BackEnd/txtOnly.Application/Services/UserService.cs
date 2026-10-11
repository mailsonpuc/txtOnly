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
        await AddUserNamesAsync(userDtos);
        return new PagedList<UserDTO>(userDtos, users.TotalCount, users.CurrentPage, users.PageSize);
    }

    public async Task<UserDTO?> GetByIdAsync(Guid? id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return null;

        var userDto = user.ToUserDTO();
        await AddUserNamesAsync([userDto]);
        return userDto;
    }

    public async Task<UserDTO> CreateAsync(UserCreateDTO user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var createdUser = await _userRepository.CreateAsync(user.ToUser());
        await _unitOfWork.CommitAsync();
        var userDto = createdUser.ToUserDTO();
        await AddUserNamesAsync([userDto]);
        return userDto;
    }

    public async Task<UserDTO> UpdateAsync(UserDTO user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var userEntity = await GetEntityByIdAsync(user.UserId);
        user.ApplyTo(userEntity);
        var updatedUser = await _userRepository.UpdateAsync(userEntity);
        await _unitOfWork.CommitAsync();
        var userDto = updatedUser.ToUserDTO();
        await AddUserNamesAsync([userDto]);
        return userDto;
    }

    private async Task AddUserNamesAsync(IReadOnlyCollection<UserDTO> users)
    {
        var names = await _userRepository.GetUserNamesAsync(users.Select(user => user.UserId));
        foreach (var user in users)
        {
            if (names.TryGetValue(user.UserId, out var userName))
                user.UserName = userName;
        }
    }

    private async Task<User> GetEntityByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O ID do usuário é obrigatório.", nameof(id));
        return await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");
    }
}