using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_Object_LocksService
{
    Task<IReadOnlyList<A_Object_Locks>> GetAllAsync();

    Task DeleteAsync(int objectLockId);
}
