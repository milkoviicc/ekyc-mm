namespace eKYC.Application.Admin;

public sealed class PermissionService : IPermissionService
{
    private readonly ICurrentUserService _currentUser;

    public PermissionService(ICurrentUserService currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        _currentUser = currentUser;
    }

    public async Task<bool> HasAnyObjectAsync(params string[] assemblyCodes)
    {
        ArgumentNullException.ThrowIfNull(assemblyCodes);

        var current = await _currentUser.GetCurrentAsync();
        return current.Permissions.Any(p => assemblyCodes.Contains(p.Asmbly_Cd, StringComparer.OrdinalIgnoreCase));
    }
}
