using DotNetEnv;
using System.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Npgsql;


// Load environment variables from .env file
Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("https://trackpulse-1w8oxsbtz-amolkumbhar87s-projects.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// builder.Services.AddScoped<IDbConnection>(sp =>
// {
//     var conn = new NpgsqlConnection(
//         builder.Configuration.GetConnectionString("DefaultConnection"));
//     conn.Open();
//     return conn;
// });

// EF Core
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        npgsqlOptions => npgsqlOptions.CommandTimeout(30)
    ));

// Dapper
builder.Services.AddSingleton<DapperContext>();

// Repositories
builder.Services.AddScoped<IHorseRepository, HorseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<JockeyRepository>();
builder.Services.AddScoped<BetRepository>();
builder.Services.AddScoped<OddsRepository>();
builder.Services.AddScoped<IRaceCardRepository, RaceCardRepository>();
builder.Services.AddScoped<IRaceRepository, RaceRepository>();
builder.Services.AddScoped<IRaceHorseRepository, RaceHorseRepository>();
builder.Services.AddScoped(typeof(RaceRepositoryBase<>), typeof(RaceRepositoryBase<>));

var app = builder.Build();


app.UseCors("AllowLocalhost");


app.MapHub<OddsHub>("/OddsHub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


app.MapControllers();

app.Run();
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
