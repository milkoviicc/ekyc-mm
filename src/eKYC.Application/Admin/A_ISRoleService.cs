using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public sealed class A_ISRoleService : IA_ISRoleService
{
    private readonly IA_ISRoleRepository _repository;

    public A_ISRoleService(IA_ISRoleRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<A_ISRole>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_ISRole?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
