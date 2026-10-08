using Dapper;
using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public sealed class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DocumentTypeRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<DocumentType>> GetAllAsync()
    {
        const string sql = """
            SELECT
                CL_Doc_Typ_Id AS ClDocTypId,
                Doc_Type_Cd   AS DocTypeCd,
                Doc_Type_Nm   AS DocTypeNm
            FROM CL_Doc_Typ
            ORDER BY Doc_Type_Nm;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<DocumentType>(sql);
        return rows.AsList();
    }
}
