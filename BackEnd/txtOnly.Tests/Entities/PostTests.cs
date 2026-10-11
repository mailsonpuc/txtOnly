using txtOnly.Domain.Entities;
using txtOnly.Domain.Validation;
using Xunit;

namespace txtOnly.Tests.Entities;

public class PostTests
{
    [Fact]
    public void Deve_Criar_Post_Com_Conteudo_Valido()
    {
        var userId = Guid.NewGuid();

        var post = new Post(userId, "  Olá, mundo!  ");

        Assert.Equal(userId, post.UserId);
        Assert.Equal("Olá, mundo!", post.Content);
        Assert.False(post.IsDeleted);
        Assert.NotEqual(default, post.CreatedAt);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_UserId_For_Vazio()
    {
        var exception = Assert.Throws<DomainExceptionValidation>(() => new Post(Guid.Empty, "conteudo"));

        Assert.Equal("UserId is required.", exception.Message);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Conteudo_For_Vazio()
    {
        var exception = Assert.Throws<DomainExceptionValidation>(() => new Post(Guid.NewGuid(), "   "));

        Assert.Equal("Content is required.", exception.Message);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Conteudo_Exceder_4000_Caracteres()
    {
        var content = new string('a', 4001);

        var exception = Assert.Throws<DomainExceptionValidation>(() => new Post(Guid.NewGuid(), content));

        Assert.Equal("Content cannot exceed 4000 characters.", exception.Message);
    }

    [Fact]
    public void Deve_Excluir_E_Restaurar_Post()
    {
        var post = new Post(Guid.NewGuid(), "Conteudo");

        post.Delete();
        Assert.True(post.IsDeleted);
        Assert.NotNull(post.UpdatedAt);

        post.Restore();
        Assert.False(post.IsDeleted);
        Assert.NotNull(post.UpdatedAt);
    }
}
