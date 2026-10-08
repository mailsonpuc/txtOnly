using txtOnly.Domain.Validation;

namespace txtOnly.Domain.Entities;

public class Like
{
    public Guid LikeId { get; private set; }

    public Guid UserId { get; private set; }

    public Guid PostId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; } = null!;

    public Post Post { get; private set; } = null!;

    private Like()
    {
    }

    public Like(Guid userId, Guid postId)
    {
        DomainExceptionValidation.When(userId == Guid.Empty, "UserId is required.");
        DomainExceptionValidation.When(postId == Guid.Empty, "PostId is required.");

        LikeId = Guid.NewGuid();
        UserId = userId;
        PostId = postId;
        CreatedAt = DateTime.UtcNow;
    }
}