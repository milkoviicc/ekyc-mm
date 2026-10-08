using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_UsrRepository
{
    Task<IReadOnlyList<A_Usr>> GetAllAsync();

    Task<A_Usr?> GetByIdAsync(int id);
}
