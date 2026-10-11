using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.Application.Mappings;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;
using txtOnly.Application.Validation;

namespace txtOnly.Application.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;

    public CommentService(IUnitOfWork unitOfWork, ICommentRepository commentRepository,
        IUserRepository userRepository, IPostRepository postRepository)
    {
        _unitOfWork = unitOfWork;
        _commentRepository = commentRepository;
        _userRepository = userRepository;
        _postRepository = postRepository;
    }

    public async Task<PagedList<CommentDTO>> GetCommentsPagedAsync(Guid postId, int pageNumber, int pageSize)
    {
        var comments = await _commentRepository.GetCommentsPagedAsync(postId, pageNumber, pageSize);
        var commentDtos = comments.Select(comment => comment.ToCommentDTO()).ToList();
        await AddUserNamesAsync(commentDtos);
        return new PagedList<CommentDTO>(commentDtos, comments.TotalCount, comments.CurrentPage, comments.PageSize);
    }

    public async Task<CommentDTO?> GetByIdAsync(Guid? id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment is null)
            return null;

        var commentDto = comment.ToCommentDTO();
        await AddUserNamesAsync([commentDto]);
        return commentDto;
    }

    public async Task<CommentDTO> CreateAsync(CommentCreateDTO comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        await EntityValidation.EnsureUserExistsAsync(_userRepository, comment.UserId);
        await EntityValidation.EnsurePostExistsAsync(_postRepository, comment.PostId);
        var createdComment = await _commentRepository.CreateAsync(comment.ToComment());
        await _unitOfWork.CommitAsync();
        var commentDto = createdComment.ToCommentDTO();
        await AddUserNamesAsync([commentDto]);
        return commentDto;
    }

    public async Task<CommentDTO> UpdateAsync(CommentDTO comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        var commentEntity = await GetEntityByIdAsync(comment.CommentId);
        comment.ApplyTo(commentEntity);
        var updatedComment = await _commentRepository.UpdateAsync(commentEntity);
        await _unitOfWork.CommitAsync();
        var commentDto = updatedComment.ToCommentDTO();
        await AddUserNamesAsync([commentDto]);
        return commentDto;
    }

    public async Task<CommentDTO> RemoveAsync(CommentDTO comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        var commentEntity = await GetEntityByIdAsync(comment.CommentId);
        var removedComment = await _commentRepository.RemoveAsync(commentEntity);
        await _unitOfWork.CommitAsync();
        var commentDto = removedComment.ToCommentDTO();
        await AddUserNamesAsync([commentDto]);
        return commentDto;
    }

    private async Task AddUserNamesAsync(IReadOnlyCollection<CommentDTO> comments)
    {
        var names = await _userRepository.GetUserNamesAsync(comments.Select(comment => comment.UserId));
        foreach (var comment in comments)
        {
            if (names.TryGetValue(comment.UserId, out var userName))
                comment.UserName = userName;
        }
    }

    private async Task<Comment> GetEntityByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O ID do comentário é obrigatório.", nameof(id));
        return await _commentRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Comentário não encontrado.");
    }

}