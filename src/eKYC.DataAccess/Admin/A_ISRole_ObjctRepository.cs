using Dapper;
using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public sealed class A_ISRole_ObjctRepository : IA_ISRole_ObjctRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public A_ISRole_ObjctRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<A_ISRole_Objct>> GetAllAsync()
    {
        const string sql = """
            SELECT ISRol_Objct_Id, ISRol_Id, Objct_Id, Objct_Ttl, Prnt_Objct_id, Sqnc_No, Mdfr_Cd, Hrchy_Lvl, Hrchy_Path
            FROM A_ISRole_Objct
            ORDER BY ISRol_Objct_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_ISRole_Objct>(sql);
        return rows.AsList();
    }

    public async Task<A_ISRole_Objct?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT ISRol_Objct_Id, ISRol_Id, Objct_Id, Objct_Ttl, Prnt_Objct_id, Sqnc_No, Mdfr_Cd, Hrchy_Lvl, Hrchy_Path
            FROM A_ISRole_Objct
            WHERE ISRol_Objct_Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_ISRole_Objct>(sql, new { Id = id });
    }
}
