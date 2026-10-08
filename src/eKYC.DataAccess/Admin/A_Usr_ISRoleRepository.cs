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

    public async Task<IReadOnlyList<A_Usr_ISRole>> GetAllAsync()
    {
        const string sql = """
            SELECT Usr_ISRol_Id, Usr_Id, ISRol_Id, Vld_From_Dt, Vld_To_Dt
            FROM A_Usr_ISRole
            ORDER BY Usr_ISRol_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Usr_ISRole>(sql);
        return rows.AsList();
    }

    public async Task<A_Usr_ISRole?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT Usr_ISRol_Id, Usr_Id, ISRol_Id, Vld_From_Dt, Vld_To_Dt
            FROM A_Usr_ISRole
            WHERE Usr_ISRol_Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_Usr_ISRole>(sql, new { Id = id });
    }
}
