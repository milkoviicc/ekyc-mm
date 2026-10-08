using eKYC.DataAccess;
using Microsoft.Extensions.Configuration;

namespace eKYC.IntegrationTests;

/// <summary>
/// Builds an <see cref="IDbConnectionFactory"/> against the real local dev database, using the same
/// user-secrets connection string this test project was configured with. These tests hit a live SQL
/// Server instance on purpose — they exist to catch SQL/column-mapping mistakes Dapper only surfaces
/// at runtime, not compile time.
/// </summary>
internal static class TestConnectionFactory
{
    public static IDbConnectionFactory Create()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(typeof(TestConnectionFactory).Assembly)
            .Build();

        return new SqlConnectionFactory(configuration);
    }
}
