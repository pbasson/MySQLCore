using MySQLCore.API.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterService(builder.Configuration);

if (!builder.Environment.IsDevelopment() ) {
    builder.RegisterSecurity();
}

var app = builder.Build();
app.RegisterApplication();
app.Run();
