using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using txtOnly.API.Controllers;
using txtOnly.API.Pagination;
using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.Domain.Pagination;

namespace txtOnly.Tests.Controllers;

public class UsersControllerTests
{
    [Fact]
    public async Task FilterByName_ReturnsPagedUsersAndForwardsFilter()
    {
        var service = new StubUserService();
        var controller = CreateController(service);

        var response = await controller.FilterByName(new UserNameFilterParameters
        {
            UserName = "Mail",
            PageNumber = 2,
            PageSize = 100
        });

        Assert.IsType<OkObjectResult>(response.Result);
        Assert.Equal("Mail", service.FilterName);
        Assert.Equal(2, service.PageNumber);
        Assert.Equal(QueryStringParameters.MaxPageSize, service.PageSize);
    }

    [Fact]
    public async Task FilterByName_ReturnsBadRequestWhenNameIsMissing()
    {
        var service = new StubUserService();
        var controller = CreateController(service);

        var response = await controller.FilterByName(new UserNameFilterParameters { UserName = " " });

        Assert.IsType<BadRequestObjectResult>(response.Result);
        Assert.Null(service.FilterName);
    }

    private static UsersController CreateController(StubUserService service) => new(service)
    {
        ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
    };

    private sealed class StubUserService : IUserService
    {
        public string? FilterName { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }

        public Task<PagedList<UserDTO>> GetUsersByNamePagedAsync(string userName, int pageNumber, int pageSize)
        {
            FilterName = userName;
            PageNumber = pageNumber;
            PageSize = pageSize;
            return Task.FromResult(new PagedList<UserDTO>([], 0, pageNumber, pageSize));
        }

        public Task<PagedList<UserDTO>> GetUsersPagedAsync(int pageNumber, int pageSize) => throw new NotSupportedException();
        public Task<UserDTO?> GetByIdAsync(Guid? id) => throw new NotSupportedException();
        public Task<UserDTO> CreateAsync(UserCreateDTO user) => throw new NotSupportedException();
        public Task<UserDTO> UpdateAsync(UserDTO user) => throw new NotSupportedException();
    }
}