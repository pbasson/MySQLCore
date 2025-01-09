using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using MySQLCore.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( x => x.SwaggerDoc("v1", new OpenApiInfo{ Title = "System", Version = "v1"}) );

builder.Services.RegisterService(builder.Configuration);
builder.Services.RegisterDatabase(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
