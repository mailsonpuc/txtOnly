using System.Security.Claims;
using Microsoft.AspNetCore.Http;
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

    [Fact]
    public async Task Create_UsesAuthenticatedUserInsteadOfRequestUserId()
    {
        var currentUserId = Guid.NewGuid();
        var spoofedUserId = Guid.NewGuid();
        var service = new StubPostService([]);
        var controller = CreateController(service, currentUserId);

        await controller.Create(new PostCreateDTO { UserId = spoofedUserId, Content = "Texto" });

        Assert.NotNull(service.CreatedPost);
        Assert.Equal(currentUserId, service.CreatedPost.UserId);
        Assert.NotEqual(spoofedUserId, service.CreatedPost.UserId);
    }

    [Fact]
    public async Task Update_ReturnsForbiddenWhenPostBelongsToAnotherUser()
    {
        var currentUserId = Guid.NewGuid();
        var service = new StubPostService([])
        {
            ExistingPost = new PostDTO { PostId = Guid.NewGuid(), UserId = Guid.NewGuid(), Content = "Original" }
        };
        var controller = CreateController(service, currentUserId);

        var response = await controller.Update(service.ExistingPost.PostId, new PostDTO
        {
            PostId = service.ExistingPost.PostId,
            UserId = currentUserId,
            Content = "Alterado"
        });

        Assert.IsType<ForbidResult>(response.Result);
        Assert.False(service.UpdateCalled);
    }

    [Fact]
    public async Task Delete_ReturnsForbiddenWhenPostBelongsToAnotherUser()
    {
        var currentUserId = Guid.NewGuid();
        var service = new StubPostService([])
        {
            ExistingPost = new PostDTO { PostId = Guid.NewGuid(), UserId = Guid.NewGuid(), Content = "Original" }
        };
        var controller = CreateController(service, currentUserId);

        var response = await controller.Delete(service.ExistingPost.PostId);

        Assert.IsType<ForbidResult>(response);
        Assert.False(service.RemoveCalled);
    }

    private static PostsController CreateController(StubPostService service, Guid userId)
    {
        var controller = new PostsController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
                        authenticationType: "test"))
                }
            }
        };

        return controller;
    }

    private sealed class StubPostService(List<PostDTO> posts) : IPostService
    {
        public PostCreateDTO? CreatedPost { get; private set; }
        public PostDTO? ExistingPost { get; init; }
        public bool UpdateCalled { get; private set; }
        public bool RemoveCalled { get; private set; }

        public Task<List<PostDTO>> GetPostsAsync() => Task.FromResult(posts);
        public Task<PagedList<PostDTO>> GetPostsPagedAsync(int pageNumber, int pageSize) => throw new NotSupportedException();
        public Task<PagedList<PostDTO>> GetPostsByUserPagedAsync(Guid userId, int pageNumber, int pageSize) => throw new NotSupportedException();
        public Task<PostDTO?> GetByIdAsync(Guid? id) => Task.FromResult(ExistingPost?.PostId == id ? ExistingPost : null);
        public Task<PostDTO> CreateAsync(PostCreateDTO post)
        {
            CreatedPost = post;
            return Task.FromResult(new PostDTO { UserId = post.UserId, Content = post.Content });
        }
        public Task<PostDTO> UpdateAsync(PostDTO post)
        {
            UpdateCalled = true;
            return Task.FromResult(post);
        }
        public Task<PostDTO> RemoveAsync(PostDTO post)
        {
            RemoveCalled = true;
            return Task.FromResult(post);
        }
        public Task<PostDTO> RestoreAsync(Guid id) => throw new NotSupportedException();
    }
}