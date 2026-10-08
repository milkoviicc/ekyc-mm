using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public sealed class A_Usr_ISRoleService : IA_Usr_ISRoleService
{
    private readonly IA_Usr_ISRoleRepository _repository;

    public A_Usr_ISRoleService(IA_Usr_ISRoleRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<A_Usr_ISRole>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_Usr_ISRole?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
