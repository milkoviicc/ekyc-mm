using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Objects (forms / menu items) each role may access (table A_ISRole_Objct).</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role, read-only for now. Rows also describe the menu hierarchy via <c>Prnt_Objct_id</c>, <c>Hrchy_Lvl</c> and <c>Hrchy_Path</c>; <c>Mdfr_Cd</c> is the access modifier code.
/// </remarks>
[ApiController]
[Route("api/admin/role-objects")]
[Authorize(Roles = "ADMIN")]
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
}
