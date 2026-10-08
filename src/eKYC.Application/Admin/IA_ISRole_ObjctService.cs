using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_ISRole_ObjctService
{
    Task<IReadOnlyList<A_ISRole_Objct>> GetAllAsync();

    Task<A_ISRole_Objct?> GetByIdAsync(int id);

    /// <summary>Grants the object (tab) to the role. Throws <see cref="eKYC.Domain.Exceptions.ValidationException"/> for unknown ids or an existing grant.</summary>
    Task<A_ISRole_Objct> GrantAsync(int roleId, int objectId);

    /// <summary>Removes a grant. Returns false if it does not exist.</summary>
    Task<bool> RevokeAsync(int id);
}
