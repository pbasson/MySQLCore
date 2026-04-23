var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterConfiguration(builder.Configuration);

var app = builder.Build();
app.RegisterApplication();
app.Run();
