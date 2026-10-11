namespace txtOnly.Application.DTOs;

public class PostCreateDTO
{
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class PostDTO
{
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}