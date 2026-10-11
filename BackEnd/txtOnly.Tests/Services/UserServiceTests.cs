using txtOnly.Application.Services;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;

namespace txtOnly.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetByIdAsync_IncludesIdentityUserName()
    {
        var userId = Guid.NewGuid();
        var repository = new StubUserRepository(new User(userId, "Bio de teste"), "mailson");
        var service = new UserService(null!, repository);

        var user = await service.GetByIdAsync(userId);

        Assert.NotNull(user);
        Assert.Equal(userId, user.UserId);
        Assert.Equal("mailson", user.UserName);
        Assert.Equal("Bio de teste", user.Bio);
    }

    private sealed class StubUserRepository(User user, string userName) : IUserRepository
    {
        public Task<User?> GetByIdAsync(Guid? id) => Task.FromResult<User?>(id == user.UserId ? user : null);

        public Task<IReadOnlyDictionary<Guid, string>> GetUserNamesAsync(IEnumerable<Guid> userIds) =>
            Task.FromResult<IReadOnlyDictionary<Guid, string>>(new Dictionary<Guid, string> { [user.UserId] = userName });

        public Task<PagedList<User>> GetUsersPagedAsync(int pageNumber, int pageSize) => throw new NotSupportedException();
        public Task<User> CreateAsync(User entity) => throw new NotSupportedException();
        public Task<User> UpdateAsync(User entity) => throw new NotSupportedException();
    }
}