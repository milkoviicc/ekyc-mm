using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Securable application objects such as forms and menu items (table A_Objct).</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role, read-only for now. <c>Objct_Typ_Id</c> references A_Objct_Typs; <c>Objct_Call</c> is the legacy Java class the object invoked.
/// </remarks>
[ApiController]
[Route("api/admin/objects")]
[Authorize(Roles = "ADMIN")]
[RequireObject("tabAdmin")]
public sealed class A_ObjctController : ControllerBase
{
    private readonly IA_ObjctService _service;

    public A_ObjctController(IA_ObjctService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    /// <summary>Lists all application objects.</summary>
    /// <response code="200">The list of rows; empty when the table has none.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<A_Objct>>> GetAll() => Ok(await _service.GetAllAsync());

    /// <summary>Gets one application object by <c>Objct_Id</c>.</summary>
    /// <param name="id">The primary key of the row.</param>
    /// <response code="200">The row.</response>
    /// <response code="404">No row with that id.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<A_Objct>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }
}
