namespace Growin.Api.Helpers;

public class ErrorPayload
{
    public required int ErrorCode { get; init; }
    public required string ErrorMessage { get; init; }

    private ErrorPayload() { }

    public static ErrorPayload New(Exception? exception, string msg, int code)
        => new()
        {
            ErrorCode = code,
            ErrorMessage = exception?.Message ?? msg,
        };
}