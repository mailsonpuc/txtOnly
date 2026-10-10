using txtOnly.Application.DTOs;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Mappings;

public static class PostDTOMappingExtensions
{
    public static PostDTO ToPostDTO(this Post post) => new()
    {
        PostId = post.PostId,
        Content = post.Content,
        CreatedAt = post.CreatedAt,
        UpdatedAt = post.UpdatedAt,
        IsDeleted = post.IsDeleted,
        UserId = post.UserId
    };

    public static Post ToPost(this PostCreateDTO post) => new(post.UserId, post.Content);

    public static void ApplyTo(this PostDTO postDTO, Post post) => post.UpdateContent(postDTO.Content);

    public static IReadOnlyList<PostDTO> ToPostDTOList(this IEnumerable<Post> posts) =>
        posts.Select(post => post.ToPostDTO()).ToList();

    public static PagedList<PostDTO> ToPostDTOPage(this PagedList<Post> posts) =>
        new(posts.ToPostDTOList().ToList(), posts.TotalCount, posts.CurrentPage, posts.PageSize);
}
