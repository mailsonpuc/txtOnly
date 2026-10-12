using System.ComponentModel.DataAnnotations;
using txtOnly.API.Pagination;

namespace txtOnly.Tests.Pagination;

public class QueryStringParametersTests
{
    [Fact]
    public void DefaultsToFirstPageAndMaximumPageSize()
    {
        var parameters = new PageQueryParameters();

        Assert.Equal(1, parameters.PageNumber);
        Assert.Equal(QueryStringParameters.MaxPageSize, parameters.PageSize);
    }

    [Fact]
    public void CapsPageSizeAtMaximum()
    {
        var parameters = new PageQueryParameters { PageSize = 100 };

        Assert.Equal(QueryStringParameters.MaxPageSize, parameters.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejectsPageNumbersBelowOne(int pageNumber)
    {
        var parameters = new PageQueryParameters { PageNumber = pageNumber };
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            parameters,
            new ValidationContext(parameters),
            validationResults,
            validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(parameters.PageNumber)));
    }
}