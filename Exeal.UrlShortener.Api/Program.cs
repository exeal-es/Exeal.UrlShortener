using Exeal.UrlShortener.Application;
using Exeal.UrlShortener.Ports.Input;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();

// Register application services
builder.Services.AddScoped<IShortUrlManager, ShortUrlManager>();
builder.Services.AddScoped<IShortUrlResolver, ShortUrlResolver>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseRouting();
app.MapControllers();

app.Run();
