using Dapper;
using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public sealed class OwnershipTypeRepository : IOwnershipTypeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OwnershipTypeRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<OwnershipType>> GetAllAsync()
    {
        const string sql = """
            SELECT
                Ownrshp_Typ_Id   AS OwnrshpTypId,
                Ownrshp_Typ_Cd   AS OwnrshpTypCd,
                Ownrshp_Typ_Dspn AS OwnrshpTypDspn
            FROM CL_Ownrshp_Typ
            ORDER BY Ownrshp_Typ_Cd;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<OwnershipType>(sql);
        return rows.AsList();
    }
}
