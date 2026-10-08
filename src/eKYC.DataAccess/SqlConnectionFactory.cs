using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace eKYC.DataAccess;

/// <inheritdoc cref="IDbConnectionFactory"/>
public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _connectionString = configuration.GetConnectionString("eKYC")
            ?? throw new InvalidOperationException("Connection string 'eKYC' is not configured.");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
