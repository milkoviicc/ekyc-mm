using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_Usr_ISRoleRepository
{
    Task<IReadOnlyList<A_Usr_ISRole>> GetAllAsync();

    Task<A_Usr_ISRole?> GetByIdAsync(int id);

    /// <summary>True if the user already holds the role today (no end date, or an end date in the future).</summary>
    Task<bool> HasActiveAssignmentAsync(int userId, int roleId);

    /// <summary>Roles the user holds today, from A_Usr_ISRole joined to A_ISRole (valid roles only).</summary>
    Task<IReadOnlyList<A_ISRole>> GetActiveRolesForUserAsync(int userId);

    /// <summary>Inserts the assignment and returns the new Usr_ISRol_Id.</summary>
    Task<int> InsertAsync(A_Usr_ISRole assignment);

    /// <summary>Soft revoke: ends the assignment now (Vld_To_Dt = now). Returns rows affected.</summary>
    Task<int> RevokeAsync(int id);
}
