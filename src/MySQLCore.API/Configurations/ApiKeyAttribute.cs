using Microsoft.AspNetCore.Mvc.Filters;

namespace MySQLCore.API.Configurations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AppSettings.API_KEY, out var apiKeyVal))
            {
                await ErrorStatus(context, 401, APIConstants.APIKey_NotFound);
            }

            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(AppSettings.API_KEY);
            if (apiKey != null && !apiKey.Equals(apiKeyVal))
            {
                await ErrorStatus(context, 403, APIConstants.APIKey_Invalid);
            }
        }

        private static async Task ErrorStatus(ActionExecutingContext context, int errorCode, string errorMsg)
        {
            context.HttpContext.Response.StatusCode = errorCode;
            await context.HttpContext.Response.WriteAsync(errorMsg);
        }
    }
}
