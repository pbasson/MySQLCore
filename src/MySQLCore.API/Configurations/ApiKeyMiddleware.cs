using ElmahCore;

namespace MySQLCore.API.Configurations;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string ApiKeyHeader = AppSettings.API_KEY;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var getApiKey = context.RequestServices.GetRequiredService<IConfiguration>().GetValue<string>(ApiKeyHeader);
            Console.WriteLine($"Test02: {getApiKey}");

            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var extractedApiKey))
            {
                Console.WriteLine($"Test01: {extractedApiKey}");
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key was not provided.");
                return;
            }


            if (!getApiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            ElmahExtensions.RaiseError(ex);
            throw;  
        }
    }
}
