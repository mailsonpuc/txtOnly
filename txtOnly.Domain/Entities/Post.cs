using txtOnly.Domain.Validation;

namespace txtOnly.Domain.Entities;

public class Post
{
    public Guid PostId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public Guid UserId { get; private set; }

    public User User { get; private set; } = null!;

    public ICollection<Comment> Comments { get; private set; } = new List<Comment>();

    public ICollection<Like> Likes { get; private set; } = new List<Like>();

    private Post()
    {
    }

    public Post(Guid userId, string content)
    {
        DomainExceptionValidation.When(userId == Guid.Empty, "UserId is required.");

        PostId = Guid.NewGuid();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdateContent(content);
    }

    public void UpdateContent(string content)
    {
        var normalizedContent = ValidateContent(content);

        Content = normalizedContent;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string ValidateContent(string content)
    {
        if (content is null)
            throw new ArgumentNullException(nameof(content));

        var normalizedContent = content.Trim();

        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(normalizedContent), "Content is required.");
        DomainExceptionValidation.When(normalizedContent.Length > 4000, "Content cannot exceed 4000 characters.");

        return normalizedContent;
    }
}