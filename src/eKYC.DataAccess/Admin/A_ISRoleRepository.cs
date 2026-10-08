using Dapper;
using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public sealed class A_ISRoleRepository : IA_ISRoleRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public A_ISRoleRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<A_ISRole>> GetAllAsync()
    {
        const string sql = """
            SELECT ISRol_Id, ISRol_Nm, ISRol_St, Apl_Cd
            FROM A_ISRole
            ORDER BY ISRol_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_ISRole>(sql);
        return rows.AsList();
    }

    public async Task<A_ISRole?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT ISRol_Id, ISRol_Nm, ISRol_St, Apl_Cd
            FROM A_ISRole
            WHERE ISRol_Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_ISRole>(sql, new { Id = id });
    }
}
