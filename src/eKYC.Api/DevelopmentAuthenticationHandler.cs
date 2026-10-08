using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace eKYC.Api;

/// <summary>
/// Auto-authenticates every request as a fixed local dev user, in place of Negotiate. Registered ONLY
/// when <c>ASPNETCORE_ENVIRONMENT=Development</c> — see Program.cs and the matching handler in
/// eKYC.Web.Blazor. Exists because Kestrel-hosted Negotiate has an unresolved multi-leg handshake bug
/// specific to Blazor Server (CLAUDE.md, decision #4) that blocks local development; without this, calls
/// from eKYC.Web.Blazor's HttpClient to this Api get a plain 401 in dev even though the Api's own
/// Negotiate setup is correct (verified working with real Windows credentials outside Blazor).
/// [Authorize]/[Authorize(Roles=...)] still run — only *how* the user is established changes.
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
