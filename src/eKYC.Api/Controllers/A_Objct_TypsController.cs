using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Lookup of application object types (table A_Objct_Typs).</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role, read-only for now.
/// </remarks>
[ApiController]
[Route("api/admin/object-types")]
[Authorize(Roles = "ADMIN")]
public sealed class A_Objct_TypsController : ControllerBase
{
    private readonly IA_Objct_TypsService _service;

    public A_Objct_TypsController(IA_Objct_TypsService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    /// <summary>Lists all object types.</summary>
    /// <response code="200">The list of rows; empty when the table has none.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<A_Objct_Typs>>> GetAll() => Ok(await _service.GetAllAsync());

    /// <summary>Gets one object type by <c>Objct_Typ_Id</c>.</summary>
    /// <param name="id">The primary key of the row.</param>
    /// <response code="200">The row.</response>
    /// <response code="404">No row with that id.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<A_Objct_Typs>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }
}
