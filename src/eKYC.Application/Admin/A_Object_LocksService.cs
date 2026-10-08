using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public sealed class A_Object_LocksService : IA_Object_LocksService
{
    private readonly IA_Object_LocksRepository _repository;

    public A_Object_LocksService(IA_Object_LocksRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<A_Object_Locks>> GetAllAsync() => _repository.GetAllAsync();

    public Task DeleteAsync(int objectLockId) => _repository.DeleteAsync(objectLockId);
}
