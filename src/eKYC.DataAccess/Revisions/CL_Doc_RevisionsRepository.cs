using Dapper;
using eKYC.Domain.Revisions;

namespace eKYC.DataAccess.Revisions;

public sealed class CL_Doc_RevisionsRepository : ICL_Doc_RevisionsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CL_Doc_RevisionsRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    // Model property names equal the column names, so no aliases are needed.
    private const string SelectColumns = """
        SELECT
            CL_Doc_Revision_Id, CL_Doc_Typ_Rev_Id, Rev_Date, Rev_Range_From, Rev_Range_To, Rev_Done_By,
            Subject, Recommendation, row_version, tenant_id
        FROM CL_Doc_Revisions
        """;

    public async Task<IReadOnlyList<CL_Doc_Revisions>> GetAllAsync(DocRevisionFilter filter, int tenantId)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var sql = new System.Text.StringBuilder(SelectColumns);
        sql.Append(" WHERE tenant_id = @tenant_id");

        var parameters = new DynamicParameters();
        parameters.Add("tenant_id", tenantId);

        if (filter.Year.HasValue)
        {
            sql.Append(" AND YEAR(Rev_Date) = @Year");
            parameters.Add("Year", filter.Year.Value);
        }

        if (filter.RevTypeId.HasValue)
        {
            sql.Append(" AND CL_Doc_Typ_Rev_Id = @RevTypeId");
            parameters.Add("RevTypeId", filter.RevTypeId.Value);
        }

        sql.Append(" ORDER BY Rev_Date DESC;");

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<CL_Doc_Revisions>(sql.ToString(), parameters);
        return rows.AsList();
    }

    public async Task<CL_Doc_Revisions?> GetByIdAsync(int docRevisionId, int tenantId)
    {
        var sql = SelectColumns + " WHERE CL_Doc_Revision_Id = @CL_Doc_Revision_Id AND tenant_id = @tenant_id;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<CL_Doc_Revisions>(
            sql, new { CL_Doc_Revision_Id = docRevisionId, tenant_id = tenantId });
    }

    public async Task<int> InsertAsync(CL_Doc_Revisions revision)
    {
        ArgumentNullException.ThrowIfNull(revision);

        const string sql = """
            INSERT INTO CL_Doc_Revisions
                (CL_Doc_Typ_Rev_Id, Rev_Date, Rev_Range_From, Rev_Range_To, Rev_Done_By, Subject, Recommendation,
                 row_version, tenant_id)
            OUTPUT INSERTED.CL_Doc_Revision_Id
            VALUES
                (@CL_Doc_Typ_Rev_Id, @Rev_Date, @Rev_Range_From, @Rev_Range_To, @Rev_Done_By, @Subject, @Recommendation,
                 1, @tenant_id);
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, revision);
    }

    public async Task<int> UpdateAsync(CL_Doc_Revisions revision)
    {
        ArgumentNullException.ThrowIfNull(revision);

        const string sql = """
            UPDATE CL_Doc_Revisions
            SET CL_Doc_Typ_Rev_Id = @CL_Doc_Typ_Rev_Id,
                Rev_Date          = @Rev_Date,
                Rev_Range_From    = @Rev_Range_From,
                Rev_Range_To      = @Rev_Range_To,
                Rev_Done_By       = @Rev_Done_By,
                Subject           = @Subject,
                Recommendation    = @Recommendation,
                row_version       = row_version + 1
            WHERE CL_Doc_Revision_Id = @CL_Doc_Revision_Id AND tenant_id = @tenant_id AND row_version = @row_version;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, revision);
    }

    public async Task<int> DeleteAsync(int docRevisionId, int tenantId)
    {
        const string sql = "DELETE FROM CL_Doc_Revisions WHERE CL_Doc_Revision_Id = @CL_Doc_Revision_Id AND tenant_id = @tenant_id;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, new { CL_Doc_Revision_Id = docRevisionId, tenant_id = tenantId });
    }
}
