using Dapper;
using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public sealed class ClientTypeRepository : IClientTypeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClientTypeRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<ClientType>> GetAllAsync()
    {
        const string sql = """
            SELECT
                Clnt_Typ_Id   AS ClntTypId,
                Clnt_Typ_Cd   AS ClntTypCd,
                Clnt_Typ_Dspn AS ClntTypDspn
            FROM CL_Clnt_Typ
            ORDER BY Clnt_Typ_Cd;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<ClientType>(sql);
        return rows.AsList();
    }
}
