namespace ContestParticipationSystem.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ApiResponse<T> SuccessResponse(T? data, string message = "") =>
        new()
        {
            Success = true,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> FailureResponse(string message) =>
        new()
        {
            Success = false,
            Message = message,
            Data = default
        };
}
