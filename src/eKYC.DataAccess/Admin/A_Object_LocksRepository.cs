using System.Data;
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

    // Age is computed with the database clock so the client never has to compare its own (possibly different) time zone.
    private const string SelectWithHolder = """
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
            U.Usr_Nm_Lst,
            DATEDIFF(SECOND, L.Locked_At, GETDATE()) AS Age_Seconds
        FROM A_Object_Locks L
        LEFT JOIN A_Usr U ON L.User_Id = U.Usr_Id
        """;

    public async Task<IReadOnlyList<A_Object_Locks>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<A_Object_Locks>(SelectWithHolder + " ORDER BY L.Locked_At DESC;");
        return rows.AsList();
    }

    public async Task<A_Object_Locks?> GetByIdAsync(int objectLockId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<A_Object_Locks>(
            SelectWithHolder + " WHERE L.Object_Lock_Id = @ObjectLockId;", new { ObjectLockId = objectLockId });
    }

    public async Task<int> DeleteAsync(int objectLockId)
    {
        const string sql = "DELETE FROM A_Object_Locks WHERE Object_Lock_Id = @ObjectLockId;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, new { ObjectLockId = objectLockId });
    }

    public async Task<AcquireLockResult> TryAcquireAsync(AcquireLockRequest request, int userId, string computerName, int ttlMinutes)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

        // UPDLOCK + HOLDLOCK keep the key range locked until commit, so two users racing for the same record
        // cannot both pass the "nobody holds it" check (this is the idempotent-insert guard, Pattern 3).
        var existing = await connection.QuerySingleOrDefaultAsync<ExistingLock>(
            """
            SELECT L.Object_Lock_Id, L.User_Id, DATEDIFF(SECOND, L.Locked_At, GETDATE()) AS Age_Seconds
            FROM A_Object_Locks L WITH (UPDLOCK, HOLDLOCK)
            WHERE L.Object_Id = @Object_Id AND L.Object_Class = @Object_Class;
            """,
            request,
            transaction);

        int lockId;
        var acquired = true;

        if (existing is null)
        {
            lockId = await connection.QuerySingleAsync<int>(
                """
                INSERT INTO A_Object_Locks (Object_Id, Object_Class, Object_Name, User_Id, Locked_At, Computer_Name)
                OUTPUT INSERTED.Object_Lock_Id
                VALUES (@Object_Id, @Object_Class, @Object_Name, @UserId, GETDATE(), @ComputerName);
                """,
                new { request.Object_Id, request.Object_Class, Object_Name = request.Object_Name ?? string.Empty, UserId = userId, ComputerName = computerName },
                transaction);
        }
        else if (existing.User_Id == userId || existing.Age_Seconds >= ttlMinutes * 60)
        {
            // Own lock (refresh) or an abandoned one (take over).
            lockId = existing.Object_Lock_Id;
            await connection.ExecuteAsync(
                """
                UPDATE A_Object_Locks
                SET User_Id = @UserId, Locked_At = GETDATE(), Computer_Name = @ComputerName,
                    Object_Name = CASE WHEN @Object_Name = '' THEN Object_Name ELSE @Object_Name END
                WHERE Object_Lock_Id = @LockId;
                """,
                new { UserId = userId, ComputerName = computerName, Object_Name = request.Object_Name ?? string.Empty, LockId = lockId },
                transaction);
        }
        else
        {
            lockId = existing.Object_Lock_Id;
            acquired = false;
        }

        var current = await connection.QuerySingleOrDefaultAsync<A_Object_Locks>(
            SelectWithHolder + " WHERE L.Object_Lock_Id = @LockId;", new { LockId = lockId }, transaction);

        transaction.Commit();
        return new AcquireLockResult { Acquired = acquired, Lock = current };
    }

    private sealed class ExistingLock
    {
        public int Object_Lock_Id { get; set; }
        public int User_Id { get; set; }
        public int Age_Seconds { get; set; }
    }

    public async Task<int> HeartbeatAsync(int objectLockId, int userId)
    {
        const string sql = "UPDATE A_Object_Locks SET Locked_At = GETDATE() WHERE Object_Lock_Id = @ObjectLockId AND User_Id = @UserId;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, new { ObjectLockId = objectLockId, UserId = userId });
    }

    public async Task<int> ReleaseAsync(int objectLockId, int userId, bool isAdmin)
    {
        const string sql = "DELETE FROM A_Object_Locks WHERE Object_Lock_Id = @ObjectLockId AND (User_Id = @UserId OR @IsAdmin = 1);";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, new { ObjectLockId = objectLockId, UserId = userId, IsAdmin = isAdmin ? 1 : 0 });
    }
}
