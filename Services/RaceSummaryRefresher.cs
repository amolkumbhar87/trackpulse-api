using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

public class RaceSummaryRefresher : BackgroundService
{
    private readonly ILogger<RaceSummaryRefresher> _logger;
    private readonly IConfiguration _configuration;
     private readonly DapperContext _dapper;

    private bool _isRunning = false;

    public RaceSummaryRefresher(
        ILogger<RaceSummaryRefresher> logger,
        IConfiguration configuration,
        DapperContext dapper)
    {
        _logger = logger;
        _configuration = configuration;
        _dapper = dapper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RaceSummaryRefresher started");

        // Initial delay (important during startup)
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        int intervalSeconds = _configuration.GetValue<int>("RaceSummaryRefreshInterval", 30);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_isRunning)
            {
                _logger.LogWarning("Previous refresh still running, skipping this cycle");
            }
            else
            {
                _isRunning = true;

                try
                {
                    using var conn = _dapper.CreateConnection();

                    _logger.LogInformation("Refreshing materialized view...");

                    await conn.ExecuteAsync(
                        "REFRESH MATERIALIZED VIEW CONCURRENTLY trackpulse.admin_race_summary");

                    _logger.LogInformation("Materialized view refreshed successfully at {time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while refreshing materialized view");
                }
                finally
                {
                    _isRunning = false;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }

        _logger.LogInformation("RaceSummaryRefresher stopped");
    }
}