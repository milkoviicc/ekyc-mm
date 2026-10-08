using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_Apl_PrmtrRepository
{
    Task<IReadOnlyList<A_Apl_Prmtr>> GetAllAsync();

    Task<A_Apl_Prmtr?> GetByIdAsync(int id);
}
