namespace MySQLCore.API.Configurations;

public static class RegisterApplications
{
    public static void RegisterApplication(this WebApplication app) 
    {
        var isDev = app.Environment.IsDevelopment();

        app.UseGlobalExceptionHandling();
        app.UseElmah();

        if (isDev) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // if(!isDev)
        // {
        //     app.UseHttpsRedirection();
        //     app.UseHsts();
        // }
        
        app.UseHttpMetrics();
        app.MapMetrics();

        app.UseMiddleware<ApiKeyMiddleware>( );
        app.UseAuthorization();
        app.MapControllers();
    }
}
