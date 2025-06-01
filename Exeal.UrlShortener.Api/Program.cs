using System.Threading.RateLimiting;
using Exeal.NotionCrm.Infra;
using Exeal.UrlShortener.Application;
using Exeal.UrlShortener.Infra;
using Exeal.UrlShortener.Ports.Input;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Exeal URL Shortener API", 
        Version = "v1",
        Description = "API para acortar URLs y gestionar estadísticas"
    });
});
builder.Services.AddCors();

// Add logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add Authentication Services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://exeal-admin.eu.auth0.com/";
    options.Audience = "https://exeal-urlshortener.onrender.com";
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("ResolveLimiter", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = builder.Configuration.GetValue<int>("RateLimiter:ResolveLimiter:PermitLimit"),
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });

    options.OnRejected = async (context, token) =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        logger.LogWarning("Rate limit exceeded for {IP}", ip);

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests.");
    };
});

// Register application services
builder.Services.AddScoped<IShortUrlManager, ShortUrlManager>();
builder.Services.AddScoped<IShortUrlResolver, ShortUrlResolver>();

// Register infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Register CRM Services
builder.Services.AddNotionCrm(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("https://www.exeal.com/"));

// Run database migrations
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Exeal URL Shortener API v1");
    });
}

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseRouting();
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();

app.Run();
