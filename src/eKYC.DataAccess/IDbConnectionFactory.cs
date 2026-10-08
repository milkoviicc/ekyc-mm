using System.Data;

namespace eKYC.DataAccess;

/// <summary>
/// Creates ADO.NET connections to the eKYC SQL Server database. Repositories take this by constructor
/// injection and open a fresh connection per call rather than holding one open across a request.
/// </summary>
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
