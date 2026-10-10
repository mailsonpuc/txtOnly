using txtOnly.Domain.Entities;
using txtOnly.Domain.Validation;
using Xunit;

namespace txtOnly.Tests.Entities;

public class CommentTests
{
    [Fact]
    public void Deve_Criar_Comentario_Com_Conteudo_Valido()
    {
        var userId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var comment = new Comment(userId, postId, "  Excelente post  ");

        Assert.Equal(userId, comment.UserId);
        Assert.Equal(postId, comment.PostId);
        Assert.Equal("Excelente post", comment.Content);
        Assert.NotEqual(default, comment.CreatedAt);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_UserId_For_Vazio()
    {
        var exception = Assert.Throws<DomainException>(() => new Comment(Guid.Empty, Guid.NewGuid(), "comentario"));

        Assert.Equal("UserId is required.", exception.Message);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_PostId_For_Vazio()
    {
        var exception = Assert.Throws<DomainException>(() => new Comment(Guid.NewGuid(), Guid.Empty, "comentario"));

        Assert.Equal("PostId is required.", exception.Message);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Conteudo_For_Invalido()
    {
        var exception = Assert.Throws<DomainException>(() => new Comment(Guid.NewGuid(), Guid.NewGuid(), "   "));

        Assert.Equal("Content is required.", exception.Message);
    }

    [Fact]
    public void Deve_Atualizar_Conteudo_Do_Comentario()
    {
        var comment = new Comment(Guid.NewGuid(), Guid.NewGuid(), "texto antigo");

        comment.UpdateContent("  texto novo  ");

        Assert.Equal("texto novo", comment.Content);
        Assert.NotNull(comment.UpdatedAt);
    }
}
