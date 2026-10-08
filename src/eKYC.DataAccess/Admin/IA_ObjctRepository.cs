using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_ObjctRepository
{
    Task<IReadOnlyList<A_Objct>> GetAllAsync();

    Task<A_Objct?> GetByIdAsync(int id);
}
