namespace txtOnly.Domain.Validation;

public class DomainExceptionValidation
{
    public static void When(bool invalidCondition, string message)
    {
        if (invalidCondition)
            throw new DomainException(message);
    }
}

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
