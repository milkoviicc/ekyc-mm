using Dapper;
using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public sealed class A_ObjctRepository : IA_ObjctRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public A_ObjctRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<A_Objct>> GetAllAsync()
    {
        const string sql = """
            SELECT Objct_Id, Objct_Typ_Id, Objct_Nm, Objct_Dspn, Objct_Call, Asmbly_Cd, Objct_St
            FROM A_Objct
            ORDER BY Objct_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Objct>(sql);
        return rows.AsList();
    }

    public async Task<A_Objct?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT Objct_Id, Objct_Typ_Id, Objct_Nm, Objct_Dspn, Objct_Call, Asmbly_Cd, Objct_St
            FROM A_Objct
            WHERE Objct_Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_Objct>(sql, new { Id = id });
    }
}
