using txtOnly.Domain.Validation;

namespace txtOnly.Domain.Entities;

public class Comment
{
    public Guid CommentId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public Guid PostId { get; private set; }

    public Guid UserId { get; private set; }

    public Post Post { get; private set; } = null!;

    public User User { get; private set; } = null!;

    private Comment()
    {
    }

    public Comment(Guid userId, Guid postId, string content)
    {
        DomainExceptionValidation.When(userId == Guid.Empty, "UserId is required.");
        DomainExceptionValidation.When(postId == Guid.Empty, "PostId is required.");

        CommentId = Guid.NewGuid();
        UserId = userId;
        PostId = postId;
        CreatedAt = DateTime.UtcNow;
        UpdateContent(content);
    }

    public void UpdateContent(string content)
    {
        var normalizedContent = ValidateContent(content);

        Content = normalizedContent;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string ValidateContent(string content)
    {
        if (content is null)
            throw new ArgumentNullException(nameof(content));

        var normalizedContent = content.Trim();

        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(normalizedContent), "Content is required.");
        DomainExceptionValidation.When(normalizedContent.Length > 500, "Content cannot exceed 500 characters.");

        return normalizedContent;
    }
}