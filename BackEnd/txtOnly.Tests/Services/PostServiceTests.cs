using txtOnly.Application.DTOs;
using txtOnly.Application.Services;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;

namespace txtOnly.Tests.Services;

public class PostServiceTests
{
    [Fact]
    public async Task GetPostsAsync_ReturnsMaterializedDtos()
    {
        var post = new Post(Guid.NewGuid(), "Texto de teste");
        var service = new PostService(null!, new StubPostRepository([post]), null!);

        var posts = await service.GetPostsAsync();

        var result = Assert.IsType<List<PostDTO>>(posts);
        var dto = Assert.Single(result);
        Assert.Equal(post.PostId, dto.PostId);
        Assert.Equal(post.Content, dto.Content);
    }

    private sealed class StubPostRepository(List<Post> posts) : IPostRepository
    {
        public Task<List<Post>> GetPostsAsync() => Task.FromResult(posts);
        public Task<PagedList<Post>> GetPostsPagedAsync(int pageNumber, int pageSize) => throw new NotSupportedException();
        public Task<PagedList<Post>> GetPostsByUserPagedAsync(Guid userId, int pageNumber, int pageSize) => throw new NotSupportedException();
        public Task<Post?> GetByIdAsync(Guid? id) => throw new NotSupportedException();
        public Task<Post> CreateAsync(Post post) => throw new NotSupportedException();
        public Task<Post> UpdateAsync(Post post) => throw new NotSupportedException();
    }
}