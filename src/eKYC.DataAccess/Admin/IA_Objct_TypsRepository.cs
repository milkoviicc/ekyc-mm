using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IA_Objct_TypsRepository
{
    Task<IReadOnlyList<A_Objct_Typs>> GetAllAsync();

    Task<A_Objct_Typs?> GetByIdAsync(int id);
}
