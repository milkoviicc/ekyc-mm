using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>User-to-role assignments with a validity window (table A_Usr_ISRole).</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role. A user can hold several roles; an assignment applies between <c>Vld_From_Dt</c> and
/// <c>Vld_To_Dt</c>. Assignments are never deleted: revoking ends them now.
/// </remarks>
[ApiController]
[Route("api/admin/user-roles")]
[Authorize(Roles = "ADMIN")]
[RequireObject("tabAdmin")]
public sealed class A_Usr_ISRoleController : ControllerBase
{
    private readonly IA_Usr_ISRoleService _service;

    public A_Usr_ISRoleController(IA_Usr_ISRoleService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    /// <summary>Lists all user-role assignments.</summary>
    /// <response code="200">The list of rows; empty when the table has none.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<A_Usr_ISRole>>> GetAll() => Ok(await _service.GetAllAsync());

    /// <summary>Gets one user-role assignment by <c>Usr_ISRol_Id</c>.</summary>
    /// <param name="id">The primary key of the row.</param>
    /// <response code="200">The row.</response>
    /// <response code="404">No row with that id.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<A_Usr_ISRole>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>Gives a user a role.</summary>
    /// <remarks>
    /// <c>Usr_Id</c> and <c>ISRol_Id</c> are required. <c>Vld_From_Dt</c> defaults to now, <c>Vld_To_Dt</c> to open-ended.
    /// A user cannot be given a role they already hold.
    /// </remarks>
    /// <response code="201">Created; the body is the stored assignment.</response>
    /// <response code="400">Unknown user or role, bad dates, or the user already holds the role.</response>
    [HttpPost]
    public async Task<ActionResult<A_Usr_ISRole>> Assign(A_Usr_ISRole assignment)
    {
        var created = await _service.AssignAsync(assignment);
        return CreatedAtAction(nameof(GetById), new { id = created.Usr_ISRol_Id }, created);
    }

    /// <summary>Ends an assignment now (soft revoke - the row stays for history).</summary>
    /// <param name="id">The assignment id.</param>
    /// <response code="204">Revoked.</response>
    /// <response code="404">No such assignment, or it has already ended.</response>
    [HttpPost("{id:int}/revoke")]
    public async Task<IActionResult> Revoke(int id) => await _service.RevokeAsync(id) ? NoContent() : NotFound();
}
