using Dapper;
using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public sealed class A_Usr_ISRoleRepository : IA_Usr_ISRoleRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public A_Usr_ISRoleRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    private const string SelectColumns = """
        SELECT Usr_ISRol_Id, Usr_Id, ISRol_Id, Vld_From_Dt, Vld_To_Dt
        FROM A_Usr_ISRole
        """;

    public async Task<IReadOnlyList<A_Usr_ISRole>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Usr_ISRole>(SelectColumns + " ORDER BY Usr_ISRol_Id;");
        return rows.AsList();
    }

    public async Task<A_Usr_ISRole?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_Usr_ISRole>(SelectColumns + " WHERE Usr_ISRol_Id = @Id;", new { Id = id });
    }

    public async Task<bool> HasActiveAssignmentAsync(int userId, int roleId)
    {
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM A_Usr_ISRole
                WHERE Usr_Id = @UserId AND ISRol_Id = @RoleId
                  AND (Vld_To_Dt IS NULL OR Vld_To_Dt > GETDATE())
            ) THEN 1 ELSE 0 END;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<bool>(sql, new { UserId = userId, RoleId = roleId });
    }

    public async Task<IReadOnlyList<A_ISRole>> GetActiveRolesForUserAsync(int userId)
    {
        const string sql = """
            SELECT DISTINCT R.ISRol_Id, R.ISRol_Nm, R.ISRol_St, R.Apl_Cd
            FROM A_Usr_ISRole UR
            JOIN A_ISRole R ON R.ISRol_Id = UR.ISRol_Id
            WHERE UR.Usr_Id = @UserId
              AND R.ISRol_St = 'V'
              AND (UR.Vld_From_Dt IS NULL OR UR.Vld_From_Dt <= GETDATE())
              AND (UR.Vld_To_Dt IS NULL OR UR.Vld_To_Dt > GETDATE())
            ORDER BY R.ISRol_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_ISRole>(sql, new { UserId = userId });
        return rows.AsList();
    }

    public async Task<int> InsertAsync(A_Usr_ISRole assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        const string sql = """
            INSERT INTO A_Usr_ISRole (Usr_Id, ISRol_Id, Vld_From_Dt, Vld_To_Dt)
            OUTPUT INSERTED.Usr_ISRol_Id
            VALUES (@Usr_Id, @ISRol_Id, @Vld_From_Dt, @Vld_To_Dt);
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, assignment);
    }

    public async Task<int> RevokeAsync(int id)
    {
        const string sql = """
            UPDATE A_Usr_ISRole
            SET Vld_To_Dt = GETDATE()
            WHERE Usr_ISRol_Id = @Id AND (Vld_To_Dt IS NULL OR Vld_To_Dt > GETDATE());
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
