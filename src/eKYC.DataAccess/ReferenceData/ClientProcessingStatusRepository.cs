using Dapper;
using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public sealed class ClientProcessingStatusRepository : IClientProcessingStatusRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClientProcessingStatusRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<ClientProcessingStatus>> GetAllAsync()
    {
        const string sql = """
            SELECT
                Clnt_PrcsSt_Id   AS ClntPrcsStId,
                Clnt_PrcsSt_Cd   AS ClntPrcsStCd,
                Status           AS Status,
                Clnt_PrcsSt_Dspn AS ClntPrcsStDspn
            FROM CL_Clnt_PrcsSt
            ORDER BY Clnt_PrcsSt_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<ClientProcessingStatus>(sql);
        return rows.AsList();
    }

    public async Task<IReadOnlyList<ClientProcessingStatusTransition>> GetAllTransitionsAsync()
    {
        const string sql = """
            SELECT
                CL_Clnt_PrcsSt_To_Id AS ClClntPrcsStToId,
                Clnt_PrcsSt_Cd_Fr    AS ClntPrcsStCdFr,
                Clnt_PrcsSt_Cd_To    AS ClntPrcsStCdTo
            FROM CL_Clnt_PrcsSt_To;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<ClientProcessingStatusTransition>(sql);
        return rows.AsList();
    }
}
