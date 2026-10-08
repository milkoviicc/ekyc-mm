using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Application configuration key/value parameters (table A_Apl_Prmtr).</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role, read-only for now. <c>Prmtr_Cd</c> is the key, <c>Prmtr_Val</c> the value.
/// </remarks>
[ApiController]
[Route("api/admin/parameters")]
[Authorize(Roles = "ADMIN")]
public sealed class A_Apl_PrmtrController : ControllerBase
{
    private readonly IA_Apl_PrmtrService _service;

    public A_Apl_PrmtrController(IA_Apl_PrmtrService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    /// <summary>Lists all application parameters.</summary>
    /// <response code="200">The list of rows; empty when the table has none.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<A_Apl_Prmtr>>> GetAll() => Ok(await _service.GetAllAsync());

    /// <summary>Gets one application parameter by <c>Apl_Id</c>.</summary>
    /// <param name="id">The primary key of the row.</param>
    /// <response code="200">The row.</response>
    /// <response code="404">No row with that id.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<A_Apl_Prmtr>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }
}
