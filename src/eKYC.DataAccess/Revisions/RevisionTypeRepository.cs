using Dapper;
using eKYC.Domain.Revisions;

namespace eKYC.DataAccess.Revisions;

public sealed class RevisionTypeRepository : IRevisionTypeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RevisionTypeRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<RevisionType>> GetAllAsync()
    {
        const string sql = """
            SELECT
                CL_Rev_Type_Id AS RevTypeId,
                Rev_Type_Cd    AS RevTypeCd,
                Rev_Type_Name  AS RevTypeName
            FROM CL_Rev_Typ
            ORDER BY Rev_Type_Name;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<RevisionType>(sql);
        return rows.AsList();
    }
}
