using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_Usr_ISRoleService
{
    Task<IReadOnlyList<A_Usr_ISRole>> GetAllAsync();

    Task<A_Usr_ISRole?> GetByIdAsync(int id);

    /// <summary>
    /// Gives a user a role from <c>Vld_From_Dt</c> (default: now) until <c>Vld_To_Dt</c> (default: open ended). Throws
    /// <see cref="eKYC.Domain.Exceptions.ValidationException"/> for unknown user/role, bad dates or a role the user already holds.
    /// </summary>
    Task<A_Usr_ISRole> AssignAsync(A_Usr_ISRole assignment);

    /// <summary>Ends an assignment now (soft revoke). Returns false if it does not exist or has already ended.</summary>
    Task<bool> RevokeAsync(int id);
}
