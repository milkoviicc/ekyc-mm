using eKYC.Application.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eKYC.Api;

/// <summary>
/// Requires that the caller's roles are granted at least one of the given application objects (A_Objct.Asmbly_Cd, e.g.
/// "tabAdmin") through A_ISRole_Objct - the legacy per-tab permission model. Checked only when
/// <c>Permissions:Enforce</c> is true, so turning it on is a deliberate step once real roles reach the API
/// (with Windows auth the role claims are AD groups, which only match the A_ISRole names after the planned role mapping).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireObjectAttribute : TypeFilterAttribute
{
    public RequireObjectAttribute(params string[] assemblyCodes)
        : base(typeof(RequireObjectFilter))
    {
        Arguments = [assemblyCodes];
    }
}

internal sealed class RequireObjectFilter : IAsyncAuthorizationFilter
{
    private readonly string[] _assemblyCodes;
    private readonly IPermissionService _permissions;
    private readonly IConfiguration _configuration;

    public RequireObjectFilter(string[] assemblyCodes, IPermissionService permissions, IConfiguration configuration)
    {
        _assemblyCodes = assemblyCodes;
        _permissions = permissions;
        _configuration = configuration;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!_configuration.GetValue<bool>("Permissions:Enforce"))
        {
            return;
        }

        // Anonymous callers are handled by [Authorize]; this filter only decides what an authenticated user may open.
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        if (!await _permissions.HasAnyObjectAsync(_assemblyCodes))
        {
            context.Result = new ObjectResult(new
            {
                error = "forbidden",
                message = $"Nemate pravo pristupa ({string.Join(", ", _assemblyCodes)}).",
            })
            {
                StatusCode = StatusCodes.Status403Forbidden,
            };
        }
    }
}
