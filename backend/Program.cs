var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();
app.UseSession();
app.MapGet("/", () => "Hello World!");

app.Run();
