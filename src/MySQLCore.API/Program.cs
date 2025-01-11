using System.Text.Json.Serialization;
using ElmahCore.Mvc;
using Microsoft.OpenApi.Models;
using MySQLCore.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( x => {
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

builder.Services.RegisterService(builder.Configuration);
builder.Services.RegisterDatabase(builder.Configuration);

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();
}

// app.UseMiddleware<ApiKeyMiddleware>( );

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseElmah();

app.Run();
