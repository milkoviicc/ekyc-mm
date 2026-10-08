using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Who is calling, with the roles and application objects (tabs) they may use.</summary>
/// <remarks>
/// The front end uses this for the user chip in the top bar and to hide screens and buttons the user cannot use.
/// It never fails for an authenticated caller: when no active A_Usr row matches the login name, <c>Matched</c> is false and the
/// roles/permissions come from the claims alone.
/// </remarks>
[ApiController]
[Route("api/me")]
[Authorize]
public sealed class MeController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public MeController(ICurrentUserService currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        _currentUser = currentUser;
    }

    /// <summary>Gets the current user.</summary>
    /// <remarks>
    /// <c>Roles</c> are what the API authorizes with today (claims). <c>DbRoles</c> are the roles recorded for the user in
    /// <c>A_Usr_ISRole</c>; they are shown for comparison until roles are read from the database. <c>Permissions</c> are the
    /// <c>A_Objct</c> rows granted to <c>Roles</c> through <c>A_ISRole_Objct</c> (the legacy tab permissions).
    /// </remarks>
    /// <response code="200">The current user.</response>
    [HttpGet]
    [ProducesResponseType<CurrentUser>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CurrentUser>> Get() => Ok(await _currentUser.GetCurrentAsync());
}
