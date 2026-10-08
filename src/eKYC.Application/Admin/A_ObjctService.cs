using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public sealed class A_ObjctService : IA_ObjctService
{
    private readonly IA_ObjctRepository _repository;

    public A_ObjctService(IA_ObjctRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<A_Objct>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_Objct?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
