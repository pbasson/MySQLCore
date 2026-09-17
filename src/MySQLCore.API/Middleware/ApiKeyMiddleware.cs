namespace MySQLCore.API.Middleware;

public sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeader = AppSettings.API_KEY;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        try 
        {
            if (context.Request.Path.StartsWithSegments("/metrics") ||
                context.Request.Path.StartsWithSegments("/health"))
            {
                await _next(context);
                return;
            }

            var getApiKey = context.RequestServices.GetRequiredService<IConfiguration>().GetValue<string>(ApiKeyHeader);

            if (string.IsNullOrWhiteSpace(getApiKey))
            {
                _logger.LogError("APIKey: API key is not configured on the server.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Server API key is not configured.");

                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var extractedApiKey))
            {
                await ErrorStatus( context, StatusCodes.Status401Unauthorized, APIConstants.APIKey_NotFound);
                _logger.LogWarning("APIKey: No API key provided in the request headers");
                return;
            }

            if (!getApiKey.Equals(extractedApiKey))
            {
                await ErrorStatus( context, StatusCodes.Status403Forbidden, APIConstants.APIKey_Invalid);
                _logger.LogWarning("APIKey: Provided API key is invalid");
                return;
            }


            await _next(context);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "APIKey: Error occurred in API Key Middleware.");
            throw;  
        }
    }

    private static async Task ErrorStatus(HttpContext context, int errorCode, string errorMsg) {
        context.Response.StatusCode = errorCode;
        await context.Response.WriteAsync(errorMsg);
    }
}
