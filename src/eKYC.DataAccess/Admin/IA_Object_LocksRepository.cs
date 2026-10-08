using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_Object_LocksRepository
{
    Task<IReadOnlyList<A_Object_Locks>> GetAllAsync();

    Task<A_Object_Locks?> GetByIdAsync(int objectLockId);

    Task<int> DeleteAsync(int objectLockId);

    /// <summary>
    /// Pattern 2 (pessimistic edit lock), atomic: takes the lock on (Object_Class, Object_Id) for the user, or reports
    /// who holds it. The user's own lock is refreshed; a lock nobody refreshed for <paramref name="ttlMinutes"/> is
    /// treated as abandoned and taken over.
    /// </summary>
    Task<AcquireLockResult> TryAcquireAsync(AcquireLockRequest request, int userId, string computerName, int ttlMinutes);

    /// <summary>Refreshes Locked_At of the user's own lock. Returns rows affected (0 = the lock is gone or someone took it over).</summary>
    Task<int> HeartbeatAsync(int objectLockId, int userId);

    /// <summary>Releases a lock; unless <paramref name="isAdmin"/> only the holder may. Returns rows affected.</summary>
    Task<int> ReleaseAsync(int objectLockId, int userId, bool isAdmin);
}
