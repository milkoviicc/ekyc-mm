using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;
using eKYC.Domain.Exceptions;

namespace eKYC.Application.Admin;

public sealed class A_Apl_PrmtrService : IA_Apl_PrmtrService
{
    private readonly IA_Apl_PrmtrRepository _repository;
    private readonly IAuditTrail _audit;

    public A_Apl_PrmtrService(IA_Apl_PrmtrRepository repository, IAuditTrail audit)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(audit);
        _repository = repository;
        _audit = audit;
    }

    public Task<IReadOnlyList<A_Apl_Prmtr>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_Apl_Prmtr?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<A_Apl_Prmtr> CreateAsync(A_Apl_Prmtr parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        parameter.Prmtr_Cd = parameter.Prmtr_Cd?.Trim();
        if (string.IsNullOrEmpty(parameter.Prmtr_Cd))
        {
            throw new ValidationException("Šifra parametra je obvezna.");
        }

        if (parameter.Prmtr_Cd.Length > 50)
        {
            throw new ValidationException("Šifra parametra može imati najviše 50 znakova.");
        }

        if (await _repository.GetByCodeAsync(parameter.Prmtr_Cd) is not null)
        {
            throw new ValidationException($"Parametar \"{parameter.Prmtr_Cd}\" već postoji.");
        }

        var id = await _repository.InsertAsync(parameter);
        await _audit.WriteAsync("A_APL_PRMTR", id, parameter.Prmtr_Cd, "INSERT", $"Novi parametar {parameter.Prmtr_Cd}");
        return await _repository.GetByIdAsync(id) ?? throw new InvalidOperationException("The created parameter could not be reloaded.");
    }

    public async Task<A_Apl_Prmtr?> UpdateAsync(int id, A_Apl_Prmtr parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return null;
        }

        var before = existing.Prmtr_Val;
        existing.Prmtr_Val = parameter.Prmtr_Val;
        existing.Prmtr_Dspn = parameter.Prmtr_Dspn;

        if (await _repository.UpdateAsync(existing) == 0)
        {
            return null; // deleted in the meantime
        }

        // The old and new value go to the audit trail: parameters drive behaviour (paths, URLs), so "who changed it from what" matters.
        await _audit.WriteAsync("A_APL_PRMTR", id, existing.Prmtr_Cd ?? string.Empty, "UPDATE", $"Vrijednost: '{before}' -> '{existing.Prmtr_Val}'");
        return await _repository.GetByIdAsync(id);
    }
}
