namespace eKYC.Application.Admin;

/// <summary>
/// The authenticated caller as the web layer sees it. Implemented in eKYC.Api on top of the HTTP context, so the
/// application layer never depends on ASP.NET.
/// </summary>
public interface ICurrentUserContext
{
    /// <summary>Login name without domain ("HBOR\Admin" becomes "Admin"); null when nobody is authenticated.</summary>
    string? LoginName { get; }

    /// <summary>Role claims of the authenticated principal.</summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>Address the request came from, stored as the lock's Computer_Name.</summary>
    string ClientHost { get; }
}
