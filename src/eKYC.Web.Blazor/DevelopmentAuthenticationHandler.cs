using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eKYC.Web.Blazor;

/// <summary>
/// Auto-authenticates every request as a fixed local dev user, in place of Negotiate. Registered ONLY
/// when <c>ASPNETCORE_ENVIRONMENT=Development</c> — see Program.cs. This exists because Kestrel-hosted
/// Negotiate has an unresolved multi-leg handshake bug specific to Blazor Server (see CLAUDE.md, decision
/// #4) that blocks local development entirely; the actual deployment target is IIS, where Windows
/// Authentication is normally handled by IIS's own module before Kestrel sees the request, so this
/// bypass should not be needed outside local dev. [Authorize]/.RequireAuthorization() still run — this
/// only changes *how* the user is established, not whether auth is checked.
/// </summary>
public sealed class DevelopmentAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "DevelopmentBypass";

    public DevelopmentAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "dev-user"),
            new Claim(ClaimTypes.Role, "UNOS"),
            new Claim(ClaimTypes.Role, "ADMIN"),
        ], SchemeName);

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
