

namespace txtOnly.Domain.Validation;

public class DeleteConstraintViolationException : Exception
{
    public DeleteConstraintViolationException(string message) : base(message)
    {
    }
}
