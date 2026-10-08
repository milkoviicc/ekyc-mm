using Dapper;
using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public sealed class RiskClassRepository : IRiskClassRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RiskClassRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<RiskClass>> GetAllAsync()
    {
        const string sql = """
            SELECT
                RskCls_Id    AS RskClsId,
                RskQstnH_Id  AS RskQstnHId,
                RskCls_Dspn  AS RskClsDspn
            FROM CL_Rsk_Cls
            ORDER BY RskCls_Dspn;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<RiskClass>(sql);
        return rows.AsList();
    }
}
