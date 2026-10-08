namespace eKYC.Domain.Exceptions;

/// <summary>Thrown by services when a request is well-formed but breaks a business rule; mapped to HTTP 400 with the message.</summary>
public sealed class ValidationException : Exception
{
    public ValidationException(string message)
        : base(message)
    {
    }
}
