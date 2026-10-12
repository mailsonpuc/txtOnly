using System.ComponentModel.DataAnnotations;

namespace txtOnly.API.Pagination;

public abstract class QueryStringParameters
{
    public const int MaxPageSize = 10;
    private int _pageSize = MaxPageSize;

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, MaxPageSize)]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}