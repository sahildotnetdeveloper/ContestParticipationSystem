namespace ContestParticipationSystem.Application.Exceptions;

public class AppException : Exception
{
    public AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
