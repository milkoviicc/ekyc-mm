using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_UsrRepository
{
    Task<IReadOnlyList<A_Usr>> GetAllAsync();

    Task<A_Usr?> GetByIdAsync(int id);

    /// <summary>Match on Lgn_Nm (case-insensitive per the database collation); prefers an active row. Null when none exists.</summary>
    Task<A_Usr?> GetByLoginNameAsync(string loginName);

    /// <summary>Inserts the user and returns the new Usr_Id.</summary>
    Task<int> InsertAsync(A_Usr user);

    /// <summary>Updates login, names, e-mail and status (last save wins - no row_version on this table). Returns rows affected; 0 means the user no longer exists.</summary>
    Task<int> UpdateAsync(A_Usr user);
}
