using Dapper;
using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public sealed class A_UsrRepository : IA_UsrRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public A_UsrRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<A_Usr>> GetAllAsync()
    {
        const string sql = """
            SELECT Usr_Id, Lgn_Nm, Usr_Nm_Fst, Usr_Nm_Lst, Pwd_Rqd_Ind, Pwd_St, Pwd_Dt, Pwd_Life, Org_Id, Usr_St, Prsn_Id, ISRol_Id, Email, Lst_Lgn_Dt, Lgn_Try_Cnt, Add_By, Add_Dt, Mdf_By, Mdf_Dt, Logged, Current_Host, SessionId
            FROM A_Usr
            ORDER BY Usr_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Usr>(sql);
        return rows.AsList();
    }

    public async Task<A_Usr?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT Usr_Id, Lgn_Nm, Usr_Nm_Fst, Usr_Nm_Lst, Pwd_Rqd_Ind, Pwd_St, Pwd_Dt, Pwd_Life, Org_Id, Usr_St, Prsn_Id, ISRol_Id, Email, Lst_Lgn_Dt, Lgn_Try_Cnt, Add_By, Add_Dt, Mdf_By, Mdf_Dt, Logged, Current_Host, SessionId
            FROM A_Usr
            WHERE Usr_Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_Usr>(sql, new { Id = id });
    }
}
