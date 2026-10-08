using Dapper;
using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public sealed class A_Objct_TypsRepository : IA_Objct_TypsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public A_Objct_TypsRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<A_Objct_Typs>> GetAllAsync()
    {
        const string sql = """
            SELECT Objct_Typ_Id, Typ_Nm, Typ_Cd
            FROM A_Objct_Typs
            ORDER BY Objct_Typ_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Objct_Typs>(sql);
        return rows.AsList();
    }

    public async Task<A_Objct_Typs?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT Objct_Typ_Id, Typ_Nm, Typ_Cd
            FROM A_Objct_Typs
            WHERE Objct_Typ_Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_Objct_Typs>(sql, new { Id = id });
    }
}
