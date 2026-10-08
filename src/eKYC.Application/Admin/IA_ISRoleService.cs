using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_ISRoleService
{
    Task<IReadOnlyList<A_ISRole>> GetAllAsync();

    Task<A_ISRole?> GetByIdAsync(int id);
}
