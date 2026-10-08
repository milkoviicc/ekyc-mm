using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;
using eKYC.Domain.Exceptions;

namespace eKYC.Application.Admin;

public sealed class A_UsrService : IA_UsrService
{
    public const string StatusActive = "V";
    public const string StatusInactive = "I";

    private readonly IA_UsrRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditTrail _audit;

    public A_UsrService(IA_UsrRepository repository, ICurrentUserService currentUser, IAuditTrail audit)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(currentUser);
        ArgumentNullException.ThrowIfNull(audit);
        _repository = repository;
        _currentUser = currentUser;
        _audit = audit;
    }

    public Task<IReadOnlyList<A_Usr>> GetAllAsync() => _repository.GetAllAsync();

    public Task<A_Usr?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<A_Usr> CreateAsync(A_Usr user)
    {
        ArgumentNullException.ThrowIfNull(user);
        Normalize(user);
        await ValidateAsync(user, existingId: null);

        var actor = await _currentUser.GetUserIdAsync() ?? 0;
        var now = DateTime.Now;

        // Defaults mirror existing rows: windows-authenticated users carry no password, so no password fields are set.
        user.Usr_St = string.IsNullOrEmpty(user.Usr_St) ? StatusActive : user.Usr_St;
        user.Pwd_Rqd_Ind = false;
        user.Pwd_St = null;
        user.Pwd_Dt = null;
        user.Pwd_Life = null;
        user.Org_Id = 0;
        user.Prsn_Id = 0;
        user.ISRol_Id = 0;
        user.Lst_Lgn_Dt = null;
        user.Lgn_Try_Cnt = 0;
        user.Add_By = actor;
        user.Add_Dt = now;
        user.Mdf_By = actor;
        user.Mdf_Dt = now;
        user.Logged = false;
        user.Current_Host = string.Empty;
        user.SessionId = null;

        var id = await _repository.InsertAsync(user);
        var created = await _repository.GetByIdAsync(id) ?? throw new InvalidOperationException("The created user could not be reloaded.");

        await _audit.WriteAsync("A_USR", id, created.Lgn_Nm ?? string.Empty, "INSERT", $"Novi korisnik {created.Lgn_Nm}");
        return created;
    }

    public async Task<A_Usr?> UpdateAsync(int id, A_Usr user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return null;
        }

        Normalize(user);
        await ValidateAsync(user, existingId: id);

        // Only the editable fields travel from the request; everything else stays as stored.
        var changed = Describe(existing, user);
        existing.Lgn_Nm = user.Lgn_Nm;
        existing.Usr_Nm_Fst = user.Usr_Nm_Fst;
        existing.Usr_Nm_Lst = user.Usr_Nm_Lst;
        existing.Email = user.Email;
        existing.Usr_St = user.Usr_St;
        existing.Mdf_By = await _currentUser.GetUserIdAsync() ?? 0;
        existing.Mdf_Dt = DateTime.Now;

        if (await _repository.UpdateAsync(existing) == 0)
        {
            return null; // deleted in the meantime
        }

        await _audit.WriteAsync("A_USR", id, existing.Lgn_Nm ?? string.Empty, "UPDATE", changed);
        return await _repository.GetByIdAsync(id);
    }

    private static void Normalize(A_Usr user)
    {
        user.Lgn_Nm = user.Lgn_Nm?.Trim();
        user.Usr_Nm_Fst = NullIfBlank(user.Usr_Nm_Fst);
        user.Usr_Nm_Lst = NullIfBlank(user.Usr_Nm_Lst);
        user.Email = NullIfBlank(user.Email);
    }

    private static string? NullIfBlank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private async Task ValidateAsync(A_Usr user, int? existingId)
    {
        if (string.IsNullOrEmpty(user.Lgn_Nm))
        {
            throw new ValidationException("Korisničko ime je obvezno.");
        }

        if (user.Lgn_Nm.Length > 20)
        {
            throw new ValidationException("Korisničko ime može imati najviše 20 znakova.");
        }

        if (user.Usr_Nm_Fst?.Length > 40 || user.Usr_Nm_Lst?.Length > 40)
        {
            throw new ValidationException("Ime i prezime mogu imati najviše 40 znakova.");
        }

        if (user.Email?.Length > 50)
        {
            throw new ValidationException("Email može imati najviše 50 znakova.");
        }

        if (!string.IsNullOrEmpty(user.Usr_St) && user.Usr_St != StatusActive && user.Usr_St != StatusInactive)
        {
            throw new ValidationException("Status korisnika mora biti V (aktivan) ili I (neaktivan).");
        }

        var sameLogin = await _repository.GetByLoginNameAsync(user.Lgn_Nm);
        if (sameLogin is not null && sameLogin.Usr_Id != existingId)
        {
            throw new ValidationException($"Korisnik s korisničkim imenom \"{user.Lgn_Nm}\" već postoji.");
        }
    }

    private static string Describe(A_Usr before, A_Usr after)
    {
        var changes = new List<string>();
        void Compare(string field, string? from, string? to)
        {
            if (!string.Equals(from ?? string.Empty, to ?? string.Empty, StringComparison.Ordinal))
            {
                changes.Add($"{field}: '{from}' -> '{to}'");
            }
        }

        Compare("Lgn_Nm", before.Lgn_Nm, after.Lgn_Nm);
        Compare("Usr_Nm_Fst", before.Usr_Nm_Fst, after.Usr_Nm_Fst);
        Compare("Usr_Nm_Lst", before.Usr_Nm_Lst, after.Usr_Nm_Lst);
        Compare("Email", before.Email, after.Email);
        Compare("Usr_St", before.Usr_St, after.Usr_St);
        return changes.Count == 0 ? "Bez promjena" : string.Join("; ", changes);
    }
}
