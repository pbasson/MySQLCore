using System.Text.Json.Serialization;
using AutoMapper;
using ElmahCore;
using ElmahCore.Mvc;
using Microsoft.OpenApi.Models;
using MySQLCore.Core.CoreHelpers;

namespace MySQLCore.API.Configurations;

public static class RegisterServices
{
    public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddEndpointsApiExplorer();

        #region Register 
        RegisterSwagger(services);
        RegisterMapping(services);
        RegisterLogs(services, configuration);
        #endregion

        #region Register DataService
        services.RegisterDataService();
        #endregion

        #region Register Database
        services.RegisterDatabase(configuration);
        #endregion


        return services;
    }

    private static void RegisterLogs(IServiceCollection services, IConfiguration configuration)
    {
        services.AddElmah<XmlFileErrorLog>(x => x.LogPath = configuration.GetValue<string>("ElmahPath"));
    }

    private static void RegisterMapping(IServiceCollection services)
    {
        var mappingConfig = new MapperConfiguration(map => map.AddProfile(new MappingProfile()));
        services.AddSingleton(mappingConfig.CreateMapper());
    }

    private static void RegisterSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen( x => {
            x.SwaggerDoc("v1", new OpenApiInfo{ Title = "MySQL Core System", Version = "v1"}); 
            x.AddSecurityDefinition(AppSettings.API_KEY, new OpenApiSecurityScheme() {
                Description = $"{AppSettings.API_KEY} Required",
                Name = AppSettings.API_KEY,
                Scheme = "ApiScheme",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });   
            x.AddSecurityRequirement(new OpenApiSecurityRequirement() { {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = AppSettings.API_KEY
                            },
                            In= ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
    }

    public static void RegisterApplication(this WebApplication app) {
        var isDev = app.Environment.IsDevelopment();

        if (isDev) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if(!isDev)
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }

        app.UseMiddleware<ApiKeyMiddleware>( );
        app.UseAuthorization();
        app.MapControllers();
        app.UseElmah();

    }
}
