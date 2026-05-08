var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterConfiguration(builder.Configuration);
builder.Host.RegisterHost();

var app = builder.Build();
app.RegisterApplication();
app.Run();
