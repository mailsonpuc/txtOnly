using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.Application.Mappings;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;
using txtOnly.Application.Validation;

namespace txtOnly.Application.Services;

public class LikeService : ILikeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILikeRepository _likeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;

    public LikeService(IUnitOfWork unitOfWork, ILikeRepository likeRepository,
        IUserRepository userRepository, IPostRepository postRepository)
    {
        _unitOfWork = unitOfWork;
        _likeRepository = likeRepository;
        _userRepository = userRepository;
        _postRepository = postRepository;
    }

    public async Task<PagedList<LikeDTO>> GetLikesPagedAsync(Guid postId, int pageNumber, int pageSize)
    {
        var likes = await _likeRepository.GetLikesPagedAsync(postId, pageNumber, pageSize);
        var likeDtos = likes.Select(like => like.ToLikeDTO()).ToList();
        return new PagedList<LikeDTO>(likeDtos, likes.TotalCount, likes.CurrentPage, likes.PageSize);
    }

    public async Task<LikeDTO> CreateAsync(LikeCreateDTO like)
    {
        ArgumentNullException.ThrowIfNull(like);
        await EntityValidation.EnsureUserExistsAsync(_userRepository, like.UserId);
        await EntityValidation.EnsurePostExistsAsync(_postRepository, like.PostId);
        if (await _likeRepository.GetByUserAndPostAsync(like.UserId, like.PostId) is not null)
            throw new InvalidOperationException("O usuário já curtiu esta publicação.");

        var createdLike = await _likeRepository.CreateAsync(like.ToLike());
        await _unitOfWork.CommitAsync();
        return createdLike.ToLikeDTO();
    }

    public async Task<LikeDTO> RemoveAsync(LikeDTO like)
    {
        ArgumentNullException.ThrowIfNull(like);
        var likeEntity = await _likeRepository.GetByUserAndPostAsync(like.UserId, like.PostId)
            ?? throw new KeyNotFoundException("Curtida não encontrada.");
        var removedLike = await _likeRepository.RemoveAsync(likeEntity);
        await _unitOfWork.CommitAsync();
        return removedLike.ToLikeDTO();
    }

}