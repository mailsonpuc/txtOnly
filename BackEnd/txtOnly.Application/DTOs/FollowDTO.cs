namespace txtOnly.Application.DTOs;

public class FollowCreateDTO
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}

public class FollowDTO
{
    public Guid FollowId { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTime CreatedAt { get; set; }
}