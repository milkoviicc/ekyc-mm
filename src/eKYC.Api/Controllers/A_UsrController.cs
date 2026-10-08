using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Application users (table A_Usr).</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role, read-only for now. The legacy <c>Pwd</c> column is never returned. Users are never hard-deleted; <c>Usr_St</c> is the status flag.
/// </remarks>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "ADMIN")]
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
}
