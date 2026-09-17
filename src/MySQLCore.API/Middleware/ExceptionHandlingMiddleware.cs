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
        FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError
    };
    
    private sealed class ErrorResponse
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; init; }

        public static ErrorResponse GetErrorResponse(Exception exception, int statusCode) => new()
        {
            StatusCode = statusCode,
            Message = statusCode == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : "One or more validation errors occurred.",
            Errors = exception is FluentValidation.ValidationException validationException
                ? validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray())
                : null
        };
    }
}