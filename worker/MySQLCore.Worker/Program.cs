var builder = Host.CreateApplicationBuilder(args);
builder.Services.RegisterConfiguration(builder.Configuration);

var host = builder.Build();
host.Run();
