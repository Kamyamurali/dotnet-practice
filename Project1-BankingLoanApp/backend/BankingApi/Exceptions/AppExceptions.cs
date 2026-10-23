namespace BankingApi.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }

    protected AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound) { }
}

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, StatusCodes.Status400BadRequest) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message, StatusCodes.Status401Unauthorized) { }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message, StatusCodes.Status403Forbidden) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, StatusCodes.Status409Conflict) { }
}
