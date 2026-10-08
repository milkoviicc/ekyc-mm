using Dapper;
using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public sealed class ClStateRepository : IClStateRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClStateRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<ClState>> GetAllAsync()
    {
        const string sql = """
            SELECT
                CL_State_Id AS ClStateId,
                State_Cd    AS StateCd,
                State_Nm    AS StateNm,
                Phone_Num   AS PhoneNum
            FROM CL_States
            ORDER BY State_Nm;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<ClState>(sql);
        return rows.AsList();
    }
}
