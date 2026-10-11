using txtOnly.Domain.Entities;
using txtOnly.Domain.Validation;
using Xunit;

namespace txtOnly.Tests.Entities;

public class LikeTests
{
    [Fact]
    public void Deve_Criar_Like_Valido()
    {
        var userId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var like = new Like(userId, postId);

        Assert.Equal(userId, like.UserId);
        Assert.Equal(postId, like.PostId);
        Assert.NotEqual(default, like.CreatedAt);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_UserId_For_Vazio()
    {
        var exception = Assert.Throws<DomainExceptionValidation>(() => new Like(Guid.Empty, Guid.NewGuid()));

        Assert.Equal("UserId is required.", exception.Message);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_PostId_For_Vazio()
    {
        var exception = Assert.Throws<DomainExceptionValidation>(() => new Like(Guid.NewGuid(), Guid.Empty));

        Assert.Equal("PostId is required.", exception.Message);
    }
}
