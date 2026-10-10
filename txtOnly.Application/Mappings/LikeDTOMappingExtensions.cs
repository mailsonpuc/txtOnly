using txtOnly.Application.DTOs;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Mappings;

public static class LikeDTOMappingExtensions
{
    public static LikeDTO ToLikeDTO(this Like like) => new()
    {
        LikeId = like.LikeId,
        UserId = like.UserId,
        PostId = like.PostId,
        CreatedAt = like.CreatedAt
    };

    public static Like ToLike(this LikeCreateDTO like) => new(like.UserId, like.PostId);

    public static IReadOnlyList<LikeDTO> ToLikeDTOList(this IEnumerable<Like> likes) =>
        likes.Select(like => like.ToLikeDTO()).ToList();

    public static PagedList<LikeDTO> ToLikeDTOPage(this PagedList<Like> likes) =>
        new(likes.ToLikeDTOList().ToList(), likes.TotalCount, likes.CurrentPage, likes.PageSize);
}