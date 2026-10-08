using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_ObjctService
{
    Task<IReadOnlyList<A_Objct>> GetAllAsync();

    Task<A_Objct?> GetByIdAsync(int id);
}
