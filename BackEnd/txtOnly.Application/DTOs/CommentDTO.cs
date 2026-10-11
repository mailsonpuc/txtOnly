namespace txtOnly.Application.DTOs;

public class CommentCreateDTO
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class CommentDTO
{
    public Guid CommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}