using txtOnly.Domain.Validation;

namespace txtOnly.Domain.Entities;

public class Follow
{
    public Guid FollowId { get; private set; }

    public Guid FollowerId { get; private set; }

    public Guid FollowingId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public User Follower { get; private set; } = null!;

    public User Following { get; private set; } = null!;

    private Follow()
    {
    }

    public Follow(Guid followerId, Guid followingId)
    {
        DomainExceptionValidation.When(followerId == Guid.Empty, "FollowerId is required.");
        DomainExceptionValidation.When(followingId == Guid.Empty, "FollowingId is required.");
        DomainExceptionValidation.When(followerId == followingId, "A user cannot follow themselves.");

        FollowId = Guid.NewGuid();
        FollowerId = followerId;
        FollowingId = followingId;
        CreatedAt = DateTime.UtcNow;
    }
}