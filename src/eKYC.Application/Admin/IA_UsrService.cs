using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_UsrService
{
    Task<IReadOnlyList<A_Usr>> GetAllAsync();

    Task<A_Usr?> GetByIdAsync(int id);
}
