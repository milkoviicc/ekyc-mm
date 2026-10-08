using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>
/// Edit-session locks held on records (table A_Object_Locks) — backs the "Zaključavanje" sub-tab of Administriranje.
/// </summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role. This is the admin view/override tool for whatever locks currently exist;
/// the lock-acquisition mechanism itself (pessimistic locking, Pattern 2 in CLAUDE.md) is not built yet.
/// </remarks>
[ApiController]
[Route("api/admin/locks")]
[Authorize(Roles = "ADMIN")]
[RequireObject("tabAdmin")]
public sealed class A_Object_LocksController : ControllerBase
{
    private readonly IA_Object_LocksService _locks;

    public A_Object_LocksController(IA_Object_LocksService locks)
    {
        ArgumentNullException.ThrowIfNull(locks);
        _locks = locks;
    }

    /// <summary>Lists all currently held locks, newest first.</summary>
    /// <remarks>
    /// Each row is joined to <c>A_Usr</c>, so it also carries the holder's login name (<c>Lgn_Nm</c>) and
    /// first/last name (<c>Usr_Nm_Fst</c>, <c>Usr_Nm_Lst</c>). The user fields are null if the lock's user no longer exists.
    /// </remarks>
    /// <response code="200">The list of locks; empty when nothing is locked.</response>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<A_Object_Locks>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<A_Object_Locks>>> GetAll() => Ok(await _locks.GetAllAsync());

    /// <summary>Force-releases a lock (admin override).</summary>
    /// <remarks>
    /// Hard-deletes the row from <c>A_Object_Locks</c> — the one place hard deletes are acceptable, since a lock is
    /// transient session state rather than business data. The user who held it is not notified and may lose unsaved work.
    /// </remarks>
    /// <param name="id">The <c>Object_Lock_Id</c> of the lock to release.</param>
    /// <response code="204">The lock was released (also returned if it was already gone).</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _locks.DeleteAsync(id);
        return NoContent();
    }
}
