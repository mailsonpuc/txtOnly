using txtOnly.Domain.Validation;

namespace txtOnly.Domain.Entities;

public class User
{
    public Guid UserId { get; private set; }

    public string Bio { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public ICollection<Post> Posts { get; private set; } = new List<Post>();

    public ICollection<Comment> Comments { get; private set; } = new List<Comment>();

    public ICollection<Like> Likes { get; private set; } = new List<Like>();

    public ICollection<Follow> Followers { get; private set; } = new List<Follow>();

    public ICollection<Follow> Following { get; private set; } = new List<Follow>();

    private User()
    {
    }

    public User(Guid userId, string bio)
    {
        DomainExceptionValidation.When(userId == Guid.Empty, "UserId is required.");

        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdateBio(bio);
    }

    public void UpdateBio(string bio)
    {
        if (bio is null)
            throw new ArgumentNullException(nameof(bio));

        var normalizedBio = string.IsNullOrWhiteSpace(bio) ? string.Empty : bio.Trim();

        DomainExceptionValidation.When(normalizedBio.Length > 200, "Bio cannot exceed 200 characters.");

        Bio = normalizedBio;
    }
}
