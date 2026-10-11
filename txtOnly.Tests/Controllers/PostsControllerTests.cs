using Microsoft.AspNetCore.Mvc;
using txtOnly.API.Controllers;
using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.Domain.Pagination;

namespace txtOnly.Tests.Controllers;

public class PostsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsServiceResults()
    {
        var posts = new List<PostDTO> { new() { Content = "Texto" } };
        var controller = new PostsController(new StubPostService(posts));

        var response = await controller.GetAll();

        var result = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(posts, result.Value);
    }

    private sealed class StubPostService(List<PostDTO> posts) : IPostService
    {
        public Task<List<PostDTO>> GetPostsAsync() => Task.FromResult(posts);
        public Task<PagedList<PostDTO>> GetPostsPagedAsync(int pageNumber, int pageSize) => throw new NotSupportedException();
        public Task<PagedList<PostDTO>> GetPostsByUserPagedAsync(Guid userId, int pageNumber, int pageSize) => throw new NotSupportedException();
        public Task<PostDTO?> GetByIdAsync(Guid? id) => throw new NotSupportedException();
        public Task<PostDTO> CreateAsync(PostCreateDTO post) => throw new NotSupportedException();
        public Task<PostDTO> UpdateAsync(PostDTO post) => throw new NotSupportedException();
        public Task<PostDTO> RemoveAsync(PostDTO post) => throw new NotSupportedException();
        public Task<PostDTO> RestoreAsync(Guid id) => throw new NotSupportedException();
    }
}