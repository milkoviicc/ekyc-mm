using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public sealed class A_UsrService : IA_UsrService
{
    private readonly IA_UsrRepository _repository;

    public A_UsrService(IA_UsrRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<A_Usr>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_Usr?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
