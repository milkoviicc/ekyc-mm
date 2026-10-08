using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public sealed class A_Objct_TypsService : IA_Objct_TypsService
{
    private readonly IA_Objct_TypsRepository _repository;

    public A_Objct_TypsService(IA_Objct_TypsRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<A_Objct_Typs>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_Objct_Typs?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
