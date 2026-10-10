using txtOnly.Application.DTOs;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Pagination;

namespace txtOnly.Application.Mappings;

public static class UserDTOMappingExtensions
{
    public static UserDTO ToUserDTO(this User user) => new()
    {
        UserId = user.UserId,
        Bio = user.Bio,
        CreatedAt = user.CreatedAt
    };

    public static User ToUser(this UserCreateDTO user) => new(user.UserId, user.Bio);

    public static void ApplyTo(this UserDTO userDTO, User user) => user.UpdateBio(userDTO.Bio);

    public static IReadOnlyList<UserDTO> ToUserDTOList(this IEnumerable<User> users) =>
        users.Select(user => user.ToUserDTO()).ToList();

    public static PagedList<UserDTO> ToUserDTOPage(this PagedList<User> users) =>
        new(users.ToUserDTOList().ToList(), users.TotalCount, users.CurrentPage, users.PageSize);
}