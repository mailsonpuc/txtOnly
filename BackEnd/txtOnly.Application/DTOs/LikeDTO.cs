namespace txtOnly.Application.DTOs;

public class LikeCreateDTO
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
}

public class LikeDTO
{
    public Guid LikeId { get; set; }
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public DateTime CreatedAt { get; set; }
}