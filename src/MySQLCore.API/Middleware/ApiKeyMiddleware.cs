namespace MySQLCore.API.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string ApiKeyHeader = AppSettings.API_KEY;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        try 
        {
            if (context.Request.Path.StartsWithSegments("/metrics"))
            {
                await _next(context);
                return;
            }

            var getApiKey = context.RequestServices.GetRequiredService<IConfiguration>() .GetValue<string>(ApiKeyHeader);

            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var extractedApiKey))
            {
                await ErrorStatus(context, 401, APIConstants.APIKey_NotFound);
                return;
            }

            if (getApiKey != null && !getApiKey.Equals(extractedApiKey))
            {
                await ErrorStatus(context, 403, APIConstants.APIKey_Invalid);
                return;
            }

            await _next(context);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred in API Key Middleware.");
            throw;  
        }
    }

    private static async Task ErrorStatus(HttpContext context, int errorCode, string errorMsg) {
        context.Response.StatusCode = errorCode;
        await context.Response.WriteAsync(errorMsg);
    }
}
