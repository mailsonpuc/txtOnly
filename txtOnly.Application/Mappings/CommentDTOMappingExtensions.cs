using txtOnly.Application.DTOs;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Mappings;

public static class CommentDTOMappingExtensions
{
    public static CommentDTO ToCommentDTO(this Comment comment) => new()
    {
        CommentId = comment.CommentId,
        Content = comment.Content,
        CreatedAt = comment.CreatedAt,
        UpdatedAt = comment.UpdatedAt,
        PostId = comment.PostId,
        UserId = comment.UserId
    };

    public static Comment ToComment(this CommentCreateDTO comment) =>
        new(comment.UserId, comment.PostId, comment.Content);

    public static void ApplyTo(this CommentDTO commentDTO, Comment comment) =>
        comment.UpdateContent(commentDTO.Content);

    public static IReadOnlyList<CommentDTO> ToCommentDTOList(this IEnumerable<Comment> comments) =>
        comments.Select(comment => comment.ToCommentDTO()).ToList();

    public static PagedList<CommentDTO> ToCommentDTOPage(this PagedList<Comment> comments) =>
        new(comments.ToCommentDTOList().ToList(), comments.TotalCount, comments.CurrentPage, comments.PageSize);
}