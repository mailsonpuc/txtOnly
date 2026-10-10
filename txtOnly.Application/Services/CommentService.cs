using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.Application.Mappings;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;

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
        return new PagedList<CommentDTO>(commentDtos, comments.TotalCount, comments.CurrentPage, comments.PageSize);
    }

    public async Task<CommentDTO?> GetByIdAsync(Guid? id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        return comment?.ToCommentDTO();
    }

    public async Task<CommentDTO> CreateAsync(CommentCreateDTO comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        await EnsureUserExistsAsync(comment.UserId);
        await EnsurePostExistsAsync(comment.PostId);
        var createdComment = await _commentRepository.CreateAsync(comment.ToComment());
        await _unitOfWork.CommitAsync();
        return createdComment.ToCommentDTO();
    }

    public async Task<CommentDTO> UpdateAsync(CommentDTO comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        var commentEntity = await GetEntityByIdAsync(comment.CommentId);
        comment.ApplyTo(commentEntity);
        var updatedComment = await _commentRepository.UpdateAsync(commentEntity);
        await _unitOfWork.CommitAsync();
        return updatedComment.ToCommentDTO();
    }

    public async Task<CommentDTO> RemoveAsync(CommentDTO comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        var commentEntity = await GetEntityByIdAsync(comment.CommentId);
        var removedComment = await _commentRepository.RemoveAsync(commentEntity);
        await _unitOfWork.CommitAsync();
        return removedComment.ToCommentDTO();
    }

    private async Task<Comment> GetEntityByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O ID do comentário é obrigatório.", nameof(id));
        return await _commentRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Comentário não encontrado.");
    }

    private async Task EnsureUserExistsAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("O usuário é obrigatório.", nameof(userId));
        if (await _userRepository.GetByIdAsync(userId) is null)
            throw new KeyNotFoundException("Usuário não encontrado.");
    }

    private async Task EnsurePostExistsAsync(Guid postId)
    {
        if (postId == Guid.Empty)
            throw new ArgumentException("A publicação é obrigatória.", nameof(postId));
        var post = await _postRepository.GetByIdAsync(postId);
        if (post is null || post.IsDeleted)
            throw new KeyNotFoundException("Publicação não encontrada.");
    }
}