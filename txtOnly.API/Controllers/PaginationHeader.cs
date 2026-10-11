using System.Text.Json;
using Microsoft.AspNetCore.Http;
using txtOnly.Domain.Pagination;

namespace txtOnly.API.Controllers;

internal static class PaginationHeader
{
    public static void Add<T>(HttpResponse response, PagedList<T> page) where T : class
    {
        response.Headers["X-Pagination"] = JsonSerializer.Serialize(new
        {
            Count = page.Count,
            PageSize = page.PageSize,
            PageCount = page.TotalPages,
            TotalItemCount = page.TotalCount,
            HasNextPage = page.HasNextPage,
            HasPreviousPage = page.HasPreviousPage
        });
    }
}