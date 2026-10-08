using Dapper;
using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public sealed class A_Object_LocksRepository : IA_Object_LocksRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public A_Object_LocksRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<A_Object_Locks>> GetAllAsync()
    {
        const string sql = """
            SELECT
                L.Object_Lock_Id,
                L.Object_Id,
                L.Object_Class,
                L.Object_Name,
                L.User_Id,
                L.Locked_At,
                L.Computer_Name,
                U.Lgn_Nm,
                U.Usr_Nm_Fst,
                U.Usr_Nm_Lst
            FROM A_Object_Locks L
            LEFT JOIN A_Usr U ON L.User_Id = U.Usr_Id
            ORDER BY L.Locked_At DESC;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Object_Locks>(sql);
        return rows.AsList();
    }

    public async Task<int> DeleteAsync(int objectLockId)
    {
        const string sql = "DELETE FROM A_Object_Locks WHERE Object_Lock_Id = @ObjectLockId;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, new { ObjectLockId = objectLockId });
    }
}
