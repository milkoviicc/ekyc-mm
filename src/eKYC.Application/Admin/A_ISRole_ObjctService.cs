using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public sealed class A_ISRole_ObjctService : IA_ISRole_ObjctService
{
    private readonly IA_ISRole_ObjctRepository _repository;

    public A_ISRole_ObjctService(IA_ISRole_ObjctRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<A_ISRole_Objct>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_ISRole_Objct?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
