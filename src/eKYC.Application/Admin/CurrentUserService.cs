using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly ICurrentUserContext _context;
    private readonly IA_UsrRepository _users;
    private readonly IA_Usr_ISRoleRepository _userRoles;
    private readonly IA_ISRole_ObjctRepository _roleObjects;

    // Scoped service, so this lives for one request: the controller, the permission filter and the audit trail
    // all ask for the current user and only the first one hits the database.
    private Task<CurrentUser>? _current;

    public CurrentUserService(
        ICurrentUserContext context,
        IA_UsrRepository users,
        IA_Usr_ISRoleRepository userRoles,
        IA_ISRole_ObjctRepository roleObjects)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(users);
        ArgumentNullException.ThrowIfNull(userRoles);
        ArgumentNullException.ThrowIfNull(roleObjects);
        _context = context;
        _users = users;
        _userRoles = userRoles;
        _roleObjects = roleObjects;
    }

    public Task<CurrentUser> GetCurrentAsync() => _current ??= LoadAsync();

    public async Task<int?> GetUserIdAsync()
    {
        var current = await GetCurrentAsync();
        return current.Matched ? current.User!.Usr_Id : null;
    }

    private async Task<CurrentUser> LoadAsync()
    {
        var loginName = _context.LoginName ?? string.Empty;
        var user = loginName.Length > 0 ? await _users.GetByLoginNameAsync(loginName) : null;
        var matched = user is not null && user.Usr_St == "V";

        var dbRoles = matched ? await _userRoles.GetActiveRolesForUserAsync(user!.Usr_Id) : [];

        // Permissions follow the roles the API actually authorizes with (the claims), mapped to A_ISRole by name.
        var permissions = await _roleObjects.GetObjectsForRoleNamesAsync(_context.Roles.ToArray());

        return new CurrentUser
        {
            LoginName = loginName,
            Matched = matched,
            User = matched ? user : null,
            Roles = _context.Roles,
            DbRoles = dbRoles,
            Permissions = permissions,
        };
    }
}
