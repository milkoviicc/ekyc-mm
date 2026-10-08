using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_Object_LocksRepository
{
    Task<IReadOnlyList<A_Object_Locks>> GetAllAsync();

    Task<int> DeleteAsync(int objectLockId);
}
