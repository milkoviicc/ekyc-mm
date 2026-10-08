using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Application users (table A_Usr).</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role. The legacy <c>Pwd</c> column is never returned or written. Users are never
/// hard-deleted: deactivate by updating <c>Usr_St</c> to <c>I</c>. Concurrent edits: the last save wins.
/// </remarks>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "ADMIN")]
[RequireObject("tabAdmin")]
public sealed class A_UsrController : ControllerBase
{
    private readonly IA_UsrService _service;

    public A_UsrController(IA_UsrService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    /// <summary>Lists all users.</summary>
    /// <response code="200">The list of rows; empty when the table has none.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<A_Usr>>> GetAll() => Ok(await _service.GetAllAsync());

    /// <summary>Gets one user by <c>Usr_Id</c>.</summary>
    /// <param name="id">The primary key of the row.</param>
    /// <response code="200">The row.</response>
    /// <response code="404">No row with that id.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<A_Usr>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>Creates an active user.</summary>
    /// <remarks>Only login name, first/last name, e-mail and status are taken from the body; the rest gets the same defaults as existing rows.</remarks>
    /// <response code="201">Created; the body is the stored user.</response>
    /// <response code="400">Missing/too long fields, bad status or a login name that already exists.</response>
    [HttpPost]
    public async Task<ActionResult<A_Usr>> Create(A_Usr user)
    {
        var created = await _service.CreateAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = created.Usr_Id }, created);
    }

    /// <summary>Updates a user (name, login, e-mail, status).</summary>
    /// <param name="id">The user id.</param>
    /// <param name="user">The user as loaded, with the edited fields.</param>
    /// <response code="200">Updated; the body is the stored user.</response>
    /// <response code="400">Invalid input or a login name that already exists.</response>
    /// <response code="404">No such user.</response>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<A_Usr>> Update(int id, A_Usr user)
    {
        var updated = await _service.UpdateAsync(id, user);
        return updated is null ? NotFound() : Ok(updated);
    }
}
