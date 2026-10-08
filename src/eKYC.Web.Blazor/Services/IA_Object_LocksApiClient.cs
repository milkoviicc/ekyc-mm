using eKYC.Domain.Admin;

namespace eKYC.Web.Blazor.Services;

public interface IA_Object_LocksApiClient
{
    Task<IReadOnlyList<A_Object_Locks>> GetAllAsync();

    Task DeleteAsync(int objectLockId);
}
