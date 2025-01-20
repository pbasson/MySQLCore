using MySQLCore.API.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterService(builder.Configuration);

var app = builder.Build();
app.RegisterApplication();
app.Run();
