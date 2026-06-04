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
        app.MapHealthChecks("/health/live");
        app.MapHealthChecks("/health/ready");

        app.UseMiddleware<ApiKeyMiddleware>( );
        app.UseAuthorization();
        app.MapControllers();
    }
}
