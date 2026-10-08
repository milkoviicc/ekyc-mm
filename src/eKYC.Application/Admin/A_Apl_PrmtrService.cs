using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public sealed class A_Apl_PrmtrService : IA_Apl_PrmtrService
{
    private readonly IA_Apl_PrmtrRepository _repository;

    public A_Apl_PrmtrService(IA_Apl_PrmtrRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<A_Apl_Prmtr>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_Apl_Prmtr?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
