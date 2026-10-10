using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.Application.Mappings;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Services;

public class FollowService : IFollowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFollowRepository _followRepository;
    private readonly IUserRepository _userRepository;

    public FollowService(IUnitOfWork unitOfWork, IFollowRepository followRepository, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _followRepository = followRepository;
        _userRepository = userRepository;
    }

    public async Task<PagedList<FollowDTO>> GetFollowersPagedAsync(Guid userId, int pageNumber, int pageSize)
    {
        var follows = await _followRepository.GetFollowersPagedAsync(userId, pageNumber, pageSize);
        var followDtos = follows.Select(follow => follow.ToFollowDTO()).ToList();
        return new PagedList<FollowDTO>(followDtos, follows.TotalCount, follows.CurrentPage, follows.PageSize);
    }

    public async Task<PagedList<FollowDTO>> GetFollowingPagedAsync(Guid userId, int pageNumber, int pageSize)
    {
        var follows = await _followRepository.GetFollowingPagedAsync(userId, pageNumber, pageSize);
        var followDtos = follows.Select(follow => follow.ToFollowDTO()).ToList();
        return new PagedList<FollowDTO>(followDtos, follows.TotalCount, follows.CurrentPage, follows.PageSize);
    }

    public async Task<FollowDTO> CreateAsync(FollowCreateDTO follow)
    {
        ArgumentNullException.ThrowIfNull(follow);
        await EnsureUserExistsAsync(follow.FollowerId);
        await EnsureUserExistsAsync(follow.FollowingId);
        if (await _followRepository.GetByUsersAsync(follow.FollowerId, follow.FollowingId) is not null)
            throw new InvalidOperationException("O usuário já segue esta conta.");

        var createdFollow = await _followRepository.CreateAsync(follow.ToFollow());
        await _unitOfWork.CommitAsync();
        return createdFollow.ToFollowDTO();
    }

    public async Task<FollowDTO> RemoveAsync(FollowDTO follow)
    {
        ArgumentNullException.ThrowIfNull(follow);
        var followEntity = await _followRepository.GetByUsersAsync(follow.FollowerId, follow.FollowingId)
            ?? throw new KeyNotFoundException("Relacionamento não encontrado.");
        var removedFollow = await _followRepository.RemoveAsync(followEntity);
        await _unitOfWork.CommitAsync();
        return removedFollow.ToFollowDTO();
    }

    private async Task EnsureUserExistsAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("O usuário é obrigatório.", nameof(userId));
        if (await _userRepository.GetByIdAsync(userId) is null)
            throw new KeyNotFoundException("Usuário não encontrado.");
    }
}