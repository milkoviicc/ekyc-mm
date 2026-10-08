using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;
using eKYC.Domain.Exceptions;

namespace eKYC.Application.Admin;

public sealed class A_ISRole_ObjctService : IA_ISRole_ObjctService
{
    private readonly IA_ISRole_ObjctRepository _repository;
    private readonly IA_ISRoleRepository _roles;
    private readonly IA_ObjctRepository _objects;
    private readonly IAuditTrail _audit;

    public A_ISRole_ObjctService(
        IA_ISRole_ObjctRepository repository,
        IA_ISRoleRepository roles,
        IA_ObjctRepository objects,
        IAuditTrail audit)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(roles);
        ArgumentNullException.ThrowIfNull(objects);
        ArgumentNullException.ThrowIfNull(audit);
        _repository = repository;
        _roles = roles;
        _objects = objects;
        _audit = audit;
    }

    public Task<IReadOnlyList<A_ISRole_Objct>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_ISRole_Objct?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<A_ISRole_Objct> GrantAsync(int roleId, int objectId)
    {
        var role = await _roles.GetByIdAsync(roleId) ?? throw new ValidationException("Uloga ne postoji.");
        var target = await _objects.GetByIdAsync(objectId) ?? throw new ValidationException("Objekt ne postoji.");

        if (await _repository.ExistsAsync(roleId, objectId))
        {
            throw new ValidationException($"Uloga {role.ISRol_Nm} već ima pravo na {target.Objct_Dspn ?? target.Objct_Nm}.");
        }

        var id = await _repository.InsertAsync(roleId, objectId);
        await _audit.WriteAsync(
            "A_ISROLE_OBJCT",
            id,
            $"{role.ISRol_Nm}:{target.Asmbly_Cd}",
            "INSERT",
            $"Ulozi {role.ISRol_Nm} dodijeljeno pravo na {target.Objct_Dspn ?? target.Objct_Nm}");

        return await _repository.GetByIdAsync(id) ?? throw new InvalidOperationException("The created grant could not be reloaded.");
    }

    public async Task<bool> RevokeAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null || await _repository.DeleteAsync(id) == 0)
        {
            return false;
        }

        var role = existing.ISRol_Id is null ? null : await _roles.GetByIdAsync(existing.ISRol_Id.Value);
        var target = existing.Objct_Id is null ? null : await _objects.GetByIdAsync(existing.Objct_Id.Value);
        await _audit.WriteAsync(
            "A_ISROLE_OBJCT",
            id,
            $"{role?.ISRol_Nm}:{target?.Asmbly_Cd}",
            "DELETE",
            $"Ulozi {role?.ISRol_Nm} oduzeto pravo na {target?.Objct_Dspn ?? target?.Objct_Nm}");
        return true;
    }
}
