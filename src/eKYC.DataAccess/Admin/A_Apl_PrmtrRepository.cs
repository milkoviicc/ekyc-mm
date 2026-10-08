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

    private const string SelectColumns = "SELECT Apl_Id, Prmtr_Cd, Prmtr_Val, Prmtr_Dspn FROM A_Apl_Prmtr";

    public async Task<IReadOnlyList<A_Apl_Prmtr>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Apl_Prmtr>(SelectColumns + " ORDER BY Apl_Id;");
        return rows.AsList();
    }

    public async Task<A_Apl_Prmtr?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_Apl_Prmtr>(SelectColumns + " WHERE Apl_Id = @Id;", new { Id = id });
    }

    public async Task<A_Apl_Prmtr?> GetByCodeAsync(string code)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<A_Apl_Prmtr>(SelectColumns + " WHERE Prmtr_Cd = @Code;", new { Code = code });
    }

    public async Task<int> InsertAsync(A_Apl_Prmtr parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        const string sql = """
            INSERT INTO A_Apl_Prmtr (Prmtr_Cd, Prmtr_Val, Prmtr_Dspn)
            OUTPUT INSERTED.Apl_Id
            VALUES (@Prmtr_Cd, @Prmtr_Val, @Prmtr_Dspn);
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, parameter);
    }

    public async Task<int> UpdateAsync(A_Apl_Prmtr parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        const string sql = """
            UPDATE A_Apl_Prmtr
            SET Prmtr_Val  = @Prmtr_Val,
                Prmtr_Dspn = @Prmtr_Dspn
            WHERE Apl_Id = @Apl_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, parameter);
    }
}
