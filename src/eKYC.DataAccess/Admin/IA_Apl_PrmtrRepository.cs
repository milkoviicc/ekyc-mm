using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_Apl_PrmtrRepository
{
    Task<IReadOnlyList<A_Apl_Prmtr>> GetAllAsync();

    Task<A_Apl_Prmtr?> GetByIdAsync(int id);

    Task<A_Apl_Prmtr?> GetByCodeAsync(string code);

    /// <summary>Inserts the parameter and returns the new Apl_Id.</summary>
    Task<int> InsertAsync(A_Apl_Prmtr parameter);

    /// <summary>Updates value and description (the code is immutable; last save wins). Returns rows affected.</summary>
    Task<int> UpdateAsync(A_Apl_Prmtr parameter);
}
