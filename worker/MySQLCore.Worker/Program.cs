var builder = Host.CreateApplicationBuilder(args);
builder.Services.RegisterConfiguration(builder.Configuration);
builder.RegisterHost();

var host = builder.Build();
host.Run();
