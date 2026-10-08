using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;
using eKYC.Domain.Exceptions;

namespace eKYC.Application.Admin;

public sealed class A_Object_LocksService : IA_Object_LocksService
{
    private readonly IA_Object_LocksRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentUserContext _context;
    private readonly ObjectLockSettings _settings;
    private readonly IAuditTrail _audit;

    public A_Object_LocksService(
        IA_Object_LocksRepository repository,
        ICurrentUserService currentUser,
        ICurrentUserContext context,
        ObjectLockSettings settings,
        IAuditTrail audit)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(currentUser);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(audit);
        _repository = repository;
        _currentUser = currentUser;
        _context = context;
        _settings = settings;
        _audit = audit;
    }

    public Task<IReadOnlyList<A_Object_Locks>> GetAllAsync() => _repository.GetAllAsync();

    public async Task DeleteAsync(int objectLockId)
    {
        var existing = await _repository.GetByIdAsync(objectLockId);
        if (await _repository.DeleteAsync(objectLockId) > 0 && existing is not null)
        {
            await _audit.WriteAsync(
                "A_OBJECT_LOCKS",
                objectLockId,
                existing.Object_Class,
                "DELETE",
                $"Administrator oslobodio zaključavanje {existing.Object_Class} #{existing.Object_Id} (korisnik {existing.Lgn_Nm})");
        }
    }

    public async Task<AcquireLockResult> AcquireAsync(AcquireLockRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Object_Class = request.Object_Class?.Trim() ?? string.Empty;
        if (request.Object_Class.Length == 0 || request.Object_Class.Length > 200)
        {
            throw new ValidationException("Klasa objekta je obvezna (najviše 200 znakova).");
        }

        if (request.Object_Name?.Length > 300)
        {
            request.Object_Name = request.Object_Name[..300];
        }

        var userId = await _currentUser.GetUserIdAsync()
            ?? throw new UnauthorizedAccessException("Za zaključavanje je potreban aktivan korisnik u sustavu (A_Usr).");

        var host = _context.ClientHost.Length > 100 ? _context.ClientHost[..100] : _context.ClientHost;
        return await _repository.TryAcquireAsync(request, userId, host, _settings.TtlMinutes);
    }

    public async Task<bool> HeartbeatAsync(int objectLockId)
    {
        var userId = await _currentUser.GetUserIdAsync();
        return userId is not null && await _repository.HeartbeatAsync(objectLockId, userId.Value) > 0;
    }

    public async Task<bool> ReleaseAsync(int objectLockId)
    {
        var current = await _currentUser.GetCurrentAsync();
        var isAdmin = current.Roles.Contains("ADMIN", StringComparer.OrdinalIgnoreCase);
        var userId = current.Matched ? current.User!.Usr_Id : -1;
        return await _repository.ReleaseAsync(objectLockId, userId, isAdmin) > 0;
    }

    public ObjectLockSettings GetSettings() => _settings;
}
