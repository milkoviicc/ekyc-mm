using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_Object_LocksService
{
    Task<IReadOnlyList<A_Object_Locks>> GetAllAsync();

    /// <summary>Admin override: removes any lock.</summary>
    Task DeleteAsync(int objectLockId);

    /// <summary>
    /// Pattern 2: asks to start editing a record. When someone else holds it the result has Acquired = false and the
    /// holder's lock. Throws <see cref="UnauthorizedAccessException"/> when the caller has no active A_Usr row.
    /// </summary>
    Task<AcquireLockResult> AcquireAsync(AcquireLockRequest request);

    /// <summary>Keeps the caller's lock alive while the editor stays open. Returns false if the lock is gone or was taken over.</summary>
    Task<bool> HeartbeatAsync(int objectLockId);

    /// <summary>Releases the caller's lock (an administrator may release any). Returns false if nothing was released.</summary>
    Task<bool> ReleaseAsync(int objectLockId);

    ObjectLockSettings GetSettings();
}
