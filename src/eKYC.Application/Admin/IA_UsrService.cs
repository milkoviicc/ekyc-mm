using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_UsrService
{
    Task<IReadOnlyList<A_Usr>> GetAllAsync();

    Task<A_Usr?> GetByIdAsync(int id);

    /// <summary>Creates an active user and returns it. Throws <see cref="eKYC.Domain.Exceptions.ValidationException"/> on bad input or a duplicate login.</summary>
    Task<A_Usr> CreateAsync(A_Usr user);

    /// <summary>
    /// Updates name, login, e-mail and status (deactivate by setting Usr_St to "I" - users are never deleted). Last save wins.
    /// Returns null if the user does not exist.
    /// </summary>
    Task<A_Usr?> UpdateAsync(int id, A_Usr user);
}
