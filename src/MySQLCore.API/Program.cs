var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterConfiguration(builder.Configuration);

// if (!builder.Environment.IsDevelopment() ) {
//     builder.RegisterSecurity();
// }

var app = builder.Build();
app.RegisterApplication();
app.Run();
