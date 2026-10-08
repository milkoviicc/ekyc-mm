using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;
using eKYC.Domain.Exceptions;

namespace eKYC.Application.Admin;

public sealed class A_Usr_ISRoleService : IA_Usr_ISRoleService
{
    private readonly IA_Usr_ISRoleRepository _repository;
    private readonly IA_UsrRepository _users;
    private readonly IA_ISRoleRepository _roles;
    private readonly IAuditTrail _audit;

    public A_Usr_ISRoleService(
        IA_Usr_ISRoleRepository repository,
        IA_UsrRepository users,
        IA_ISRoleRepository roles,
        IAuditTrail audit)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(users);
        ArgumentNullException.ThrowIfNull(roles);
        ArgumentNullException.ThrowIfNull(audit);
        _repository = repository;
        _users = users;
        _roles = roles;
        _audit = audit;
    }

    public Task<IReadOnlyList<A_Usr_ISRole>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_Usr_ISRole?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<A_Usr_ISRole> AssignAsync(A_Usr_ISRole assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (assignment.Usr_Id is null || assignment.ISRol_Id is null)
        {
            throw new ValidationException("Korisnik i uloga su obvezni.");
        }

        var user = await _users.GetByIdAsync(assignment.Usr_Id.Value)
            ?? throw new ValidationException("Korisnik ne postoji.");
        var role = await _roles.GetByIdAsync(assignment.ISRol_Id.Value)
            ?? throw new ValidationException("Uloga ne postoji.");

        assignment.Vld_From_Dt ??= DateTime.Now;
        if (assignment.Vld_To_Dt is not null && assignment.Vld_To_Dt <= assignment.Vld_From_Dt)
        {
            throw new ValidationException("Datum završetka mora biti nakon datuma početka.");
        }

        if (await _repository.HasActiveAssignmentAsync(user.Usr_Id, role.ISRol_Id))
        {
            throw new ValidationException($"Korisnik {user.Lgn_Nm} već ima ulogu {role.ISRol_Nm}.");
        }

        var id = await _repository.InsertAsync(assignment);
        await _audit.WriteAsync(
            "A_USR_ISROLE",
            id,
            $"{user.Lgn_Nm}:{role.ISRol_Nm}",
            "INSERT",
            $"Dodijeljena uloga {role.ISRol_Nm} korisniku {user.Lgn_Nm} od {assignment.Vld_From_Dt:dd.MM.yyyy.}" +
            (assignment.Vld_To_Dt is null ? string.Empty : $" do {assignment.Vld_To_Dt:dd.MM.yyyy.}"));

        return await _repository.GetByIdAsync(id) ?? throw new InvalidOperationException("The created assignment could not be reloaded.");
    }

    public async Task<bool> RevokeAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null || await _repository.RevokeAsync(id) == 0)
        {
            return false;
        }

        var user = existing.Usr_Id is null ? null : await _users.GetByIdAsync(existing.Usr_Id.Value);
        var role = existing.ISRol_Id is null ? null : await _roles.GetByIdAsync(existing.ISRol_Id.Value);
        await _audit.WriteAsync(
            "A_USR_ISROLE",
            id,
            $"{user?.Lgn_Nm}:{role?.ISRol_Nm}",
            "UPDATE",
            $"Ukinuta uloga {role?.ISRol_Nm} korisniku {user?.Lgn_Nm}");
        return true;
    }
}
