using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_Usr_ISRoleService
{
    Task<IReadOnlyList<A_Usr_ISRole>> GetAllAsync();

    Task<A_Usr_ISRole?> GetByIdAsync(int id);
}
