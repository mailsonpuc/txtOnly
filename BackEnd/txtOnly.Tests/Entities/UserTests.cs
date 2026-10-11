using txtOnly.Domain.Entities;
using txtOnly.Domain.Validation;
using Xunit;

namespace txtOnly.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Deve_Criar_Usuario_Com_Bio_Valida()
    {
        var userId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        var user = new User(userId, "  Minha bio  ");

        var after = DateTime.UtcNow;
        Assert.Equal(userId, user.UserId);
        Assert.Equal("Minha bio", user.Bio);
        Assert.True(user.CreatedAt >= before && user.CreatedAt <= after);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_UserId_For_Vazio()
    {
        var exception = Assert.Throws<DomainExceptionValidation>(() => new User(Guid.Empty, "bio"));

        Assert.Equal("UserId is required.", exception.Message);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Bio_For_Nula()
    {
        var user = new User(Guid.NewGuid(), "bio");

        Assert.Throws<ArgumentNullException>(() => user.UpdateBio(null!));
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Bio_Exceder_200_Caracteres()
    {
        var user = new User(Guid.NewGuid(), "bio");
        var bio = new string('x', 201);

        var exception = Assert.Throws<DomainExceptionValidation>(() => user.UpdateBio(bio));

        Assert.Equal("Bio cannot exceed 200 characters.", exception.Message);
    }
}
