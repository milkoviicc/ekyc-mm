using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_ISRoleRepository
{
    Task<IReadOnlyList<A_ISRole>> GetAllAsync();

    Task<A_ISRole?> GetByIdAsync(int id);
}
