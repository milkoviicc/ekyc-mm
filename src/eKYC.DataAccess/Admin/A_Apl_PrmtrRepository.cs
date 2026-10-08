using Dapper;
using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public sealed class A_Apl_PrmtrRepository : IA_Apl_PrmtrRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public A_Apl_PrmtrRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<A_Apl_Prmtr>> GetAllAsync()
    {
        const string sql = """
            SELECT Apl_Id, Prmtr_Cd, Prmtr_Val, Prmtr_Dspn
            FROM A_Apl_Prmtr
            ORDER BY Apl_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Apl_Prmtr>(sql);
        return rows.AsList();
    }

    public async Task<A_Apl_Prmtr?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT Apl_Id, Prmtr_Cd, Prmtr_Val, Prmtr_Dspn
            FROM A_Apl_Prmtr
            WHERE Apl_Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_Apl_Prmtr>(sql, new { Id = id });
    }
}
