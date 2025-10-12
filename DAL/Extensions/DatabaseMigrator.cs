using DAL.Configs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DAL.Extensions
{
    public static class DatabaseMigrator
    {
        public static async Task EnsureDatabasesAndMigrationsAppliedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<object>>();

            try
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var databaseConfig = configuration.GetSection("Database").Get<DatabaseConfig>();

                if (string.IsNullOrEmpty(databaseConfig.MoneyManagerDB)
                    || string.IsNullOrEmpty(databaseConfig.HangfireDB))
                {
                    logger.LogWarning("Database connection string is not configured");
                    return;
                }

                await EnsureDatabaseExistsAsync(databaseConfig.HangfireDB, "HangfireDB", logger);

                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                await dbContext.Database.MigrateAsync();

                logger.LogInformation("Database migrations applied successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database migration failed");
                throw;
            }
        }

        private static async Task EnsureDatabaseExistsAsync(string connectionString, string databaseName, ILogger logger)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                var originalDatabase = builder.InitialCatalog;

                builder.InitialCatalog = "master";

                using var connection = new SqlConnection(builder.ConnectionString);
                await connection.OpenAsync();

                var commandText = $@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{originalDatabase}')
                    BEGIN
                        CREATE DATABASE [{originalDatabase}];
                        PRINT 'Database {originalDatabase} created successfully';
                    END
                    ELSE
                        PRINT 'Database {originalDatabase} already exists';";

                using var command = new SqlCommand(commandText, connection);
                await command.ExecuteNonQueryAsync();

                logger.LogInformation($"Database {originalDatabase} ensured");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to ensure database {databaseName} exists");
                throw;
            }
        }
    }
}
