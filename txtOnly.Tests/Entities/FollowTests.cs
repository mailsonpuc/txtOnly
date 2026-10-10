using txtOnly.Domain.Entities;
using txtOnly.Domain.Validation;
using Xunit;

namespace txtOnly.Tests.Entities;

public class FollowTests
{
    [Fact]
    public void Deve_Criar_Seguimento_Valido()
    {
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();

        var follow = new Follow(followerId, followingId);

        Assert.Equal(followerId, follow.FollowerId);
        Assert.Equal(followingId, follow.FollowingId);
        Assert.NotEqual(default, follow.CreatedAt);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_FollowerId_For_Vazio()
    {
        var exception = Assert.Throws<DomainException>(() => new Follow(Guid.Empty, Guid.NewGuid()));

        Assert.Equal("FollowerId is required.", exception.Message);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_FollowingId_For_Vazio()
    {
        var exception = Assert.Throws<DomainException>(() => new Follow(Guid.NewGuid(), Guid.Empty));

        Assert.Equal("FollowingId is required.", exception.Message);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Usuario_Tentar_Seguirem_Si_Mesmo()
    {
        var userId = Guid.NewGuid();

        var exception = Assert.Throws<DomainException>(() => new Follow(userId, userId));

        Assert.Equal("A user cannot follow themselves.", exception.Message);
    }
}
