using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_Apl_PrmtrService
{
    Task<IReadOnlyList<A_Apl_Prmtr>> GetAllAsync();

    Task<A_Apl_Prmtr?> GetByIdAsync(int id);
}
