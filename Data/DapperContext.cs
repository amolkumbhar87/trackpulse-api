// Data/DapperContext.cs
using System.Data;
using Npgsql;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? configuration.GetConnectionString("DefaultConnection")!;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}