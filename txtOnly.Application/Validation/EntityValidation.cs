using txtOnly.Domain.Interfaces;

namespace txtOnly.Application.Validation;

public static class EntityValidation
{
    public static async Task EnsureUserExistsAsync(IUserRepository userRepository, Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("O usuário é obrigatório.", nameof(userId));
        if (await userRepository.GetByIdAsync(userId) is null)
            throw new KeyNotFoundException("Usuário não encontrado.");
    }

    public static async Task EnsurePostExistsAsync(IPostRepository postRepository, Guid postId)
    {
        if (postId == Guid.Empty)
            throw new ArgumentException("A publicação é obrigatória.", nameof(postId));
        var post = await postRepository.GetByIdAsync(postId);
        if (post is null || post.IsDeleted)
            throw new KeyNotFoundException("Publicação não encontrada.");
    }
}