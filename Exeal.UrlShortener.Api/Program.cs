using Exeal.UrlShortener.Application;
using Exeal.UrlShortener.Infra;
using Exeal.UrlShortener.Ports.Input;
using Exeal.UrlShortener.Ports.Output;
using FluentMigrator.Runner;
using Microsoft.OpenApi.Models;

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

// Register application services
builder.Services.AddScoped<IShortUrlManager, ShortUrlManager>();
builder.Services.AddScoped<IShortUrlResolver, ShortUrlResolver>();

// Register infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

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
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseRouting();
app.MapControllers();

app.Run();
