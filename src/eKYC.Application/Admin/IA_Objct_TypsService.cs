using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IA_Objct_TypsService
{
    Task<IReadOnlyList<A_Objct_Typs>> GetAllAsync();

    Task<A_Objct_Typs?> GetByIdAsync(int id);
}
