namespace MySQLCore.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware( RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");

            ElmahCore.ElmahExtensions.RaiseError(exception);

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode = GetStatusCode(exception);
        context.Response.StatusCode = statusCode;

        ErrorResponse response = ErrorResponse.GetErrorResponse(exception, statusCode);

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        ValidationException => (int)HttpStatusCode.BadRequest,
        // NotFoundException => (int)HttpStatusCode.NotFound,
        UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
        // DataPersistenceException => (int)HttpStatusCode.InternalServerError,
        _ => (int)HttpStatusCode.InternalServerError
    };

    private sealed class ErrorResponse
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;

        public static ErrorResponse GetErrorResponse(Exception exception, int statusCode) => new()
        {
            StatusCode = statusCode,
            Message = statusCode == (int)HttpStatusCode.InternalServerError ? "An unexpected error occurred." : exception.Message
        };
    }
}