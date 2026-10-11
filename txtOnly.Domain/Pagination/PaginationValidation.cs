namespace txtOnly.Domain.Pagination;

public static class PaginationValidation
{
    public const int MaximumPageSize = 100;

    public static void Validate(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "A página deve ser maior que zero.");
        if (pageSize <= 0 || pageSize > MaximumPageSize)
            throw new ArgumentOutOfRangeException(nameof(pageSize), $"O tamanho da página deve estar entre 1 e {MaximumPageSize}.");
    }
}