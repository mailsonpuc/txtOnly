using txtOnly.Domain.Pagination;

namespace txtOnly.Tests.Pagination;

public class PaginationValidationTests
{
    [Fact]
    public void Validate_RejectsPageSizeOverMaximum()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => PaginationValidation.Validate(1, PaginationValidation.MaximumPageSize + 1));

        Assert.Equal("pageSize", exception.ParamName);
    }

    [Fact]
    public void Validate_AllowsMaximumPageSize()
    {
        PaginationValidation.Validate(1, PaginationValidation.MaximumPageSize);
    }
}