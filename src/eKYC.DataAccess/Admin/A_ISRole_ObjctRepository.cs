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

    private const string SelectColumns = """
        SELECT ISRol_Objct_Id, ISRol_Id, Objct_Id, Objct_Ttl, Prnt_Objct_id, Sqnc_No, Mdfr_Cd, Hrchy_Lvl, Hrchy_Path
        FROM A_ISRole_Objct
        """;

    public async Task<IReadOnlyList<A_ISRole_Objct>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_ISRole_Objct>(SelectColumns + " ORDER BY ISRol_Objct_Id;");
        return rows.AsList();
    }

    public async Task<A_ISRole_Objct?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_ISRole_Objct>(SelectColumns + " WHERE ISRol_Objct_Id = @Id;", new { Id = id });
    }

    public async Task<bool> ExistsAsync(int roleId, int objectId)
    {
        const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM A_ISRole_Objct WHERE ISRol_Id = @RoleId AND Objct_Id = @ObjectId) THEN 1 ELSE 0 END;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<bool>(sql, new { RoleId = roleId, ObjectId = objectId });
    }

    public async Task<IReadOnlyList<A_Objct>> GetObjectsForRoleNamesAsync(IReadOnlyCollection<string> roleNames)
    {
        ArgumentNullException.ThrowIfNull(roleNames);
        if (roleNames.Count == 0)
        {
            return [];
        }

        const string sql = """
            SELECT DISTINCT O.Objct_Id, O.Objct_Typ_Id, O.Objct_Nm, O.Objct_Dspn, O.Objct_Call, O.Asmbly_Cd, O.Objct_St
            FROM A_ISRole_Objct RO
            JOIN A_ISRole R ON R.ISRol_Id = RO.ISRol_Id
            JOIN A_Objct O ON O.Objct_Id = RO.Objct_Id
            WHERE R.ISRol_Nm IN @RoleNames
              AND R.ISRol_St = 'V'
              AND O.Objct_St = 'V'
            ORDER BY O.Objct_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Objct>(sql, new { RoleNames = roleNames });
        return rows.AsList();
    }

    public async Task<int> InsertAsync(int roleId, int objectId)
    {
        // Mirrors the legacy rows: title = object name, flat hierarchy, blank modifier code.
        const string sql = """
            INSERT INTO A_ISRole_Objct (ISRol_Id, Objct_Id, Objct_Ttl, Prnt_Objct_id, Sqnc_No, Mdfr_Cd, Hrchy_Lvl, Hrchy_Path)
            OUTPUT INSERTED.ISRol_Objct_Id
            SELECT @RoleId, O.Objct_Id, O.Objct_Nm, 0, 0, '  ', 0, ''
            FROM A_Objct O
            WHERE O.Objct_Id = @ObjectId;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, new { RoleId = roleId, ObjectId = objectId });
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync("DELETE FROM A_ISRole_Objct WHERE ISRol_Objct_Id = @Id;", new { Id = id });
    }
}
