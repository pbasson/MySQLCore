using MySQLCore.API.Configurations;
using MySQLCore.Core.CoreHelpers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterService(builder.Configuration);
builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(7840, listenOptions => { listenOptions.UseHttps(); }); });

var app = builder.Build();
app.RegisterApplication();
app.Run();
