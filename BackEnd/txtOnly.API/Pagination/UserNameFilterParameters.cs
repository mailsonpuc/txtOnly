namespace txtOnly.API.Pagination;

public sealed class UserNameFilterParameters : QueryStringParameters
{
    public string? UserName { get; set; }
}