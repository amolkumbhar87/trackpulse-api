using System.Text;
using EvolveDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

public static class ServiceCollection
{

    public static void ApplyDbMigrations(this IApplicationBuilder app, string connectionString)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DBMigration");
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        var evolve = new Evolve(connection, msg => logger.LogInformation(msg))
        {
            Locations = new[] { "DB/DataMigration" },
            IsEraseDisabled = true,
            MetadataTableName = "trackpulse_changelog"
        };

        evolve.Migrate();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IHorseRepository, HorseRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJockeyRepository, JockeyRepository>();
        services.AddScoped<IBetRepository, BetRepository>();
        services.AddScoped<IOddsRepository, OddsRepository>();
        services.AddScoped<IRaceCardRepository, RaceCardRepository>();
        services.AddScoped<IRaceRepository, RaceRepository>();
        services.AddScoped<IRaceHorseRepository, RaceHorseRepository>();
        services.AddScoped<IRaceDayRepository, RaceDayRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<IDepositRepository, DepositRepository>();
        services.AddScoped<ITrainerRepository, TrainerRepository>();

        services.AddScoped(typeof(RaceRepositoryBase<>), typeof(RaceRepositoryBase<>));
    }

    public static void AddSwagger(this IServiceCollection services)
    {


    }

    public static void JWTAuthentication(this IServiceCollection services, IConfiguration configuration)
    {

        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
        services.AddAuthentication(options =>
        {

            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        // Optional: Clock skew (useful in development)
        ClockSkew = TimeSpan.Zero
    };
});

    }
}