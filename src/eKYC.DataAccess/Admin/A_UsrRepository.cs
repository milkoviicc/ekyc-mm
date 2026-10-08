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

    // Pwd is deliberately never selected (varchar(20) plaintext-era column, see CLAUDE.md).
    private const string SelectColumns = """
        SELECT Usr_Id, Lgn_Nm, Usr_Nm_Fst, Usr_Nm_Lst, Pwd_Rqd_Ind, Pwd_St, Pwd_Dt, Pwd_Life, Org_Id, Usr_St, Prsn_Id, ISRol_Id, Email, Lst_Lgn_Dt, Lgn_Try_Cnt, Add_By, Add_Dt, Mdf_By, Mdf_Dt, Logged, Current_Host, SessionId
        FROM A_Usr
        """;

    public async Task<IReadOnlyList<A_Usr>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Usr>(SelectColumns + " ORDER BY Usr_Id;");
        return rows.AsList();
    }

    public async Task<A_Usr?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_Usr>(SelectColumns + " WHERE Usr_Id = @Id;", new { Id = id });
    }

    public async Task<A_Usr?> GetByLoginNameAsync(string loginName)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<A_Usr>(
            SelectColumns + " WHERE Lgn_Nm = @LoginName ORDER BY CASE WHEN Usr_St = 'V' THEN 0 ELSE 1 END, Usr_Id;",
            new { LoginName = loginName });
    }

    public async Task<int> InsertAsync(A_Usr user)
    {
        ArgumentNullException.ThrowIfNull(user);

        const string sql = """
            INSERT INTO A_Usr
                (Lgn_Nm, Usr_Nm_Fst, Usr_Nm_Lst, Pwd_Rqd_Ind, Pwd_St, Pwd_Dt, Pwd_Life, Org_Id, Usr_St, Prsn_Id, ISRol_Id, Email,
                 Lst_Lgn_Dt, Lgn_Try_Cnt, Add_By, Add_Dt, Mdf_By, Mdf_Dt, Logged, Current_Host, SessionId)
            OUTPUT INSERTED.Usr_Id
            VALUES
                (@Lgn_Nm, @Usr_Nm_Fst, @Usr_Nm_Lst, @Pwd_Rqd_Ind, @Pwd_St, @Pwd_Dt, @Pwd_Life, @Org_Id, @Usr_St, @Prsn_Id, @ISRol_Id, @Email,
                 @Lst_Lgn_Dt, @Lgn_Try_Cnt, @Add_By, @Add_Dt, @Mdf_By, @Mdf_Dt, @Logged, @Current_Host, @SessionId);
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, user);
    }

    public async Task<int> UpdateAsync(A_Usr user)
    {
        ArgumentNullException.ThrowIfNull(user);

        const string sql = """
            UPDATE A_Usr
            SET Lgn_Nm      = @Lgn_Nm,
                Usr_Nm_Fst  = @Usr_Nm_Fst,
                Usr_Nm_Lst  = @Usr_Nm_Lst,
                Email       = @Email,
                Usr_St      = @Usr_St,
                Mdf_By      = @Mdf_By,
                Mdf_Dt      = @Mdf_Dt
            WHERE Usr_Id = @Usr_Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, user);
    }
}
