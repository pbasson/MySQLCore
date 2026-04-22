using ElmahCore;

namespace MySQLCore.API.Configurations;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string ApiKeyHeader = AppSettings.API_KEY;

    public ApiKeyMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        try {
            var getApiKey = context.RequestServices.GetRequiredService<IConfiguration>().GetValue<string>(ApiKeyHeader);

            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var extractedApiKey)) {
                await ErrorStatus(context, 401, APIConstants.APIKey_NotFound);
                return;
            }

            if (getApiKey != null && !getApiKey.Equals(extractedApiKey)) {
                await ErrorStatus(context, 403, APIConstants.APIKey_Invalid);
                return;
            }

            await _next(context);
        }
        catch (Exception ex) {
            ElmahExtensions.RaiseError(ex);
            throw;  
        }
    }

    private static async Task ErrorStatus(HttpContext context, int errorCode, string errorMsg) {
        context.Response.StatusCode = errorCode;
        await context.Response.WriteAsync(errorMsg);
    }
}
