var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapControllers();

app.Run();