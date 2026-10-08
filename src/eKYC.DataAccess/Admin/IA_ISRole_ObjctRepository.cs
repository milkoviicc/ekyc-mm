using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_ISRole_ObjctRepository
{
    Task<IReadOnlyList<A_ISRole_Objct>> GetAllAsync();

    Task<A_ISRole_Objct?> GetByIdAsync(int id);
}
