using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_Apl_PrmtrService
{
    Task<IReadOnlyList<A_Apl_Prmtr>> GetAllAsync();

    Task<A_Apl_Prmtr?> GetByIdAsync(int id);

    /// <summary>Adds a parameter. The code must be unique and is immutable afterwards.</summary>
    Task<A_Apl_Prmtr> CreateAsync(A_Apl_Prmtr parameter);

    /// <summary>
    /// Changes value and description (last save wins). Returns null if it does not exist.
    /// </summary>
    Task<A_Apl_Prmtr?> UpdateAsync(int id, A_Apl_Prmtr parameter);
}
