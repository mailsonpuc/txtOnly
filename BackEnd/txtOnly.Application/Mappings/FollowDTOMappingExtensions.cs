using txtOnly.Application.DTOs;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Mappings;

public static class FollowDTOMappingExtensions
{
    public static FollowDTO ToFollowDTO(this Follow follow) => new()
    {
        FollowId = follow.FollowId,
        FollowerId = follow.FollowerId,
        FollowingId = follow.FollowingId,
        CreatedAt = follow.CreatedAt
    };

    public static Follow ToFollow(this FollowCreateDTO follow) => new(follow.FollowerId, follow.FollowingId);

    public static IReadOnlyList<FollowDTO> ToFollowDTOList(this IEnumerable<Follow> follows) =>
        follows.Select(follow => follow.ToFollowDTO()).ToList();

    public static PagedList<FollowDTO> ToFollowDTOPage(this PagedList<Follow> follows) =>
        new(follows.ToFollowDTOList().ToList(), follows.TotalCount, follows.CurrentPage, follows.PageSize);
}