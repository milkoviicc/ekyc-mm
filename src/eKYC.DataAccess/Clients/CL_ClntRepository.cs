using Dapper;
using eKYC.Domain.Clients;

namespace eKYC.DataAccess.Clients;

public sealed class CL_ClntRepository : ICL_ClntRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CL_ClntRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    // Model property names equal the column names, so no aliases are needed.
    private const string SelectColumns = """
        SELECT
            Clnt_Id, HBOR_ID, Clnt_Typ_Cd, Clnt_St, Clnt_Prcsng_St, RskEst_Id, Rsk_Pnts, PEP_Ind, Rmrk, WtchLst_Ind,
            Add_By, Add_Dt, Mdf_By, Mdf_Dt, row_version, tenant_id
        FROM CL_Clnt
        """;

    public async Task<CL_Clnt?> GetByIdAsync(int clntId, int tenantId)
    {
        var sql = SelectColumns + " WHERE Clnt_Id = @Clnt_Id AND tenant_id = @tenant_id;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<CL_Clnt>(sql, new { Clnt_Id = clntId, tenant_id = tenantId });
    }

    public async Task<IReadOnlyList<CL_Clnt>> GetAllAsync(int tenantId)
    {
        var sql = SelectColumns + " WHERE tenant_id = @tenant_id ORDER BY Clnt_Id;";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<CL_Clnt>(sql, new { tenant_id = tenantId });
        return rows.AsList();
    }

    public async Task<int> InsertAsync(CL_Clnt client)
    {
        ArgumentNullException.ThrowIfNull(client);

        const string sql = """
            INSERT INTO CL_Clnt
                (HBOR_ID, Clnt_Typ_Cd, Clnt_St, Clnt_Prcsng_St, RskEst_Id, Rsk_Pnts, PEP_Ind, Rmrk, WtchLst_Ind,
                 Add_By, Add_Dt, Mdf_By, Mdf_Dt, row_version, tenant_id)
            OUTPUT INSERTED.Clnt_Id
            VALUES
                (@HBOR_ID, @Clnt_Typ_Cd, @Clnt_St, @Clnt_Prcsng_St, @RskEst_Id, @Rsk_Pnts, @PEP_Ind, @Rmrk, @WtchLst_Ind,
                 @Add_By, @Add_Dt, @Mdf_By, @Mdf_Dt, 1, @tenant_id);
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, client);
    }

    public async Task<int> UpdateAsync(CL_Clnt client)
    {
        ArgumentNullException.ThrowIfNull(client);

        const string sql = """
            UPDATE CL_Clnt
            SET Clnt_Typ_Cd    = @Clnt_Typ_Cd,
                Clnt_St        = @Clnt_St,
                Clnt_Prcsng_St = @Clnt_Prcsng_St,
                RskEst_Id      = @RskEst_Id,
                Rsk_Pnts       = @Rsk_Pnts,
                PEP_Ind        = @PEP_Ind,
                Rmrk           = @Rmrk,
                WtchLst_Ind    = @WtchLst_Ind,
                Mdf_By         = @Mdf_By,
                Mdf_Dt         = @Mdf_Dt,
                row_version    = row_version + 1
            WHERE Clnt_Id = @Clnt_Id AND tenant_id = @tenant_id AND row_version = @row_version;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, client);
    }
}
