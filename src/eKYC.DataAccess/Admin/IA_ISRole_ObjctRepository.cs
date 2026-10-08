using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_ISRole_ObjctRepository
{
    Task<IReadOnlyList<A_ISRole_Objct>> GetAllAsync();

    Task<A_ISRole_Objct?> GetByIdAsync(int id);

    Task<bool> ExistsAsync(int roleId, int objectId);

    /// <summary>
    /// Application objects (tabs) granted to roles with the given names (A_ISRole.ISRol_Nm); valid roles and valid
    /// objects only, one row per object.
    /// </summary>
    Task<IReadOnlyList<A_Objct>> GetObjectsForRoleNamesAsync(IReadOnlyCollection<string> roleNames);

    /// <summary>Grants an object to a role (copying the legacy row shape) and returns the new ISRol_Objct_Id.</summary>
    Task<int> InsertAsync(int roleId, int objectId);

    /// <summary>
    /// Revokes a grant. The table has no status column, so this is a real delete - the only hard delete among the A_ tables.
    /// Returns rows affected.
    /// </summary>
    Task<int> DeleteAsync(int id);
}
