using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_ISRole_ObjctService
{
    Task<IReadOnlyList<A_ISRole_Objct>> GetAllAsync();

    Task<A_ISRole_Objct?> GetByIdAsync(int id);
}
