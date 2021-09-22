using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PersonProfile_DF.Api.Middlewares
{
    public class HealthCheckDbMiddleware : IHealthCheck
    {
        private string _connectionString = null;

        public HealthCheckDbMiddleware(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch(Exception exp)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Database health-check failed", exp));
            }
        }
    }
}


