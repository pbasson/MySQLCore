namespace MySQLCore.API.Configurations;

public static class RegisterApplications
{
    public static void RegisterApplication(this WebApplication app) 
    {
        var isDev = app.Environment.IsDevelopment();

        app.UseGlobalExceptionHandling();

        if (isDev) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();

        // if(!isDev)
        // {
        //     app.UseHttpsRedirection();
        //     app.UseHsts();
        // }
        
        app.UseHttpMetrics();
        app.MapMetrics();

        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

        app.UseCors("Frontend");
        app.UseRateLimiter();
        app.UseMiddleware<ApiKeyMiddleware>( );
        app.UseAuthorization();
        app.MapControllers().RequireRateLimiting("fixed");
    }
}
