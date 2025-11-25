using backend.Service;
using backend.Exceptions;
using StackExchange.Redis;
using backend.Configurations;
using backend.Util;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(redisConnection);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "Spotify-Tidal-Session";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "Spotify-Tidal.Session";
});
builder.Services.AddSingleton<TokenEncryptor>();

builder.Services.Configure<TidalAuthOptions>(
    builder.Configuration.GetSection("TidalAuthOptions"));
builder.Services.Configure<SpotifyAuthOptions>(
    builder.Configuration.GetSection("SpotifyAuthOptions"));

builder.Services.AddControllers();

builder.Services.AddCors((options) =>
{
    options.AddPolicy("AllowAll", policy =>
{
    policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
});
});
builder.Services.AddHttpClient();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISpotifyService, SpotifyService>();
builder.Services.AddScoped<ITidalService, TidalService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}
app.UseRouting();

app.UseCors("AllowAll");

app.UseSession();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
