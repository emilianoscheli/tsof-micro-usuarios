namespace UsuariosAPI.WebAPI.Services
{
    using Microsoft.Data.SqlClient;

    public class LogCleanupService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LogCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromDays(1);
        private readonly int _retentionMonths;

        public LogCleanupService(IConfiguration configuration, ILogger<LogCleanupService> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._retentionMonths = configuration.GetValue("LogRetentionMonths", 1);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await this.CleanupOldLogsAsync();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Error al limpiar logs antiguos");
                }

                await Task.Delay(this._cleanupInterval, stoppingToken);
            }
        }

        private async Task CleanupOldLogsAsync()
        {
            var connectionString = this._configuration.GetConnectionString("DBConnectionString");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(
                "DELETE FROM [dbo].[Logs] WHERE [TimeStamp] < DATEADD(MONTH, @Months, GETDATE())",
                connection);

            command.Parameters.AddWithValue("@Months", -this._retentionMonths);

            var deleted = await command.ExecuteNonQueryAsync();
            this._logger.LogInformation("Se eliminaron {Count} logs antiguos (>{Months} meses)", deleted, this._retentionMonths);
        }
    }
}