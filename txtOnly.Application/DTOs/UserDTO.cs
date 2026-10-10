namespace txtOnly.Application.DTOs;

public class UserCreateDTO
{
    public Guid UserId { get; set; }
    public string Bio { get; set; } = string.Empty;
}

public class UserDTO
{
    public Guid UserId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}