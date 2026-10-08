namespace eKYC.Domain.Admin;

/// <summary>
/// Who is calling: the authenticated identity, the matching A_Usr row (if any), the roles the API authorizes with
/// (claims) next to the roles recorded in A_Usr_ISRole, and the application objects (tabs) those roles may open.
/// </summary>
public sealed class CurrentUser
{
    /// <summary>Login name without domain, e.g. "Admin" for "HBOR\Admin".</summary>
    public string LoginName { get; set; } = string.Empty;

    /// <summary>True when an active A_Usr row exists for <see cref="LoginName"/>.</summary>
    public bool Matched { get; set; }

    public A_Usr? User { get; set; }

    /// <summary>Roles on the authenticated principal - what [Authorize(Roles=...)] checks today.</summary>
    public IReadOnlyList<string> Roles { get; set; } = [];

    /// <summary>Roles assigned in A_Usr_ISRole that are valid today (informational until roles are read from the database).</summary>
    public IReadOnlyList<A_ISRole> DbRoles { get; set; } = [];

    /// <summary>A_Objct rows (tabs) granted to <see cref="Roles"/> through A_ISRole_Objct.</summary>
    public IReadOnlyList<A_Objct> Permissions { get; set; } = [];
}
