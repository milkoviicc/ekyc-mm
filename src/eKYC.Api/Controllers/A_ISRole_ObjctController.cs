using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Objects (tabs) each role may access (table A_ISRole_Objct).</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role. Rows also describe the menu hierarchy via <c>Prnt_Objct_id</c>, <c>Hrchy_Lvl</c> and
/// <c>Hrchy_Path</c>; <c>Mdfr_Cd</c> is the access modifier code (blank in all existing data).
/// </remarks>
[ApiController]
[Route("api/admin/role-objects")]
[Authorize(Roles = "ADMIN")]
[RequireObject("tabAdmin")]
public sealed class A_ISRole_ObjctController : ControllerBase
{
    private readonly IA_ISRole_ObjctService _service;

    public A_ISRole_ObjctController(IA_ISRole_ObjctService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    /// <summary>Lists all role-object permissions.</summary>
    /// <response code="200">The list of rows; empty when the table has none.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<A_ISRole_Objct>>> GetAll() => Ok(await _service.GetAllAsync());

    /// <summary>Gets one role-object permission by <c>ISRol_Objct_Id</c>.</summary>
    /// <param name="id">The primary key of the row.</param>
    /// <response code="200">The row.</response>
    /// <response code="404">No row with that id.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<A_ISRole_Objct>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>Grants a role the right to open an object (tab).</summary>
    /// <remarks>Only <c>ISRol_Id</c> and <c>Objct_Id</c> are read from the body; the rest copies the shape of existing rows.</remarks>
    /// <response code="201">Created; the body is the stored grant.</response>
    /// <response code="400">Unknown role or object, or the role already has the right.</response>
    [HttpPost]
    public async Task<ActionResult<A_ISRole_Objct>> Grant(A_ISRole_Objct grant)
    {
        if (grant.ISRol_Id is null || grant.Objct_Id is null)
        {
            return BadRequest(new { error = "validation_error", message = "ISRol_Id i Objct_Id su obvezni." });
        }

        var created = await _service.GrantAsync(grant.ISRol_Id.Value, grant.Objct_Id.Value);
        return CreatedAtAction(nameof(GetById), new { id = created.ISRol_Objct_Id }, created);
    }

    /// <summary>Takes a right away from a role.</summary>
    /// <remarks>The table has no status column, so this deletes the row. Every change is written to the audit trail.</remarks>
    /// <param name="id">The <c>ISRol_Objct_Id</c> of the grant.</param>
    /// <response code="204">Revoked.</response>
    /// <response code="404">No such grant.</response>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Revoke(int id) => await _service.RevokeAsync(id) ? NoContent() : NotFound();
}
