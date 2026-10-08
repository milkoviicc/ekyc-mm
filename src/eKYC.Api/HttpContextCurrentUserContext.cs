using System.Security.Claims;
using eKYC.Application.Admin;

namespace eKYC.Api;

/// <summary>Reads the caller from the ASP.NET Core HTTP context (Negotiate/Windows in production, the dev handler locally).</summary>
public sealed class HttpContextCurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public HttpContextCurrentUserContext(IHttpContextAccessor accessor)
    {
        ArgumentNullException.ThrowIfNull(accessor);
        _accessor = accessor;
    }

    private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;

    public string? LoginName
    {
        get
        {
            var name = Principal?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            // Windows names arrive as "DOMAIN\user" (or user@domain); A_Usr.Lgn_Nm holds just the account name.
            var slash = name.LastIndexOf('\\');
            if (slash >= 0)
            {
                name = name[(slash + 1)..];
            }

            var at = name.IndexOf('@');
            return at > 0 ? name[..at] : name;
        }
    }

    public IReadOnlyList<string> Roles =>
        Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() ?? [];

    public string ClientHost => _accessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
