using EvolveDb;
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
            Locations = new[] { "db/migrations" },
            IsEraseDisabled = true,
            MetadataTableName = "trackpulse_changelog"
        };

        evolve.Migrate();
    }
}