using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Application configuration key/value parameters (table A_Apl_Prmtr).</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role. <c>Prmtr_Cd</c> is the key (immutable), <c>Prmtr_Val</c> the value. Updates record the old and new
/// value in the audit trail (the last save wins).
/// </remarks>
[ApiController]
[Route("api/admin/parameters")]
[Authorize(Roles = "ADMIN")]
[RequireObject("tabAdmin")]
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

    /// <summary>Adds an application parameter.</summary>
    /// <response code="201">Created; the body is the stored parameter.</response>
    /// <response code="400">Missing or too long code, or the code already exists.</response>
    [HttpPost]
    public async Task<ActionResult<A_Apl_Prmtr>> Create(A_Apl_Prmtr parameter)
    {
        var created = await _service.CreateAsync(parameter);
        return CreatedAtAction(nameof(GetById), new { id = created.Apl_Id }, created);
    }

    /// <summary>Changes a parameter's value and description.</summary>
    /// <param name="id">The parameter id.</param>
    /// <param name="parameter">The parameter as loaded, with the edited value/description.</param>
    /// <response code="200">Updated; the body is the stored parameter.</response>
    /// <response code="404">No such parameter.</response>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<A_Apl_Prmtr>> Update(int id, A_Apl_Prmtr parameter)
    {
        var updated = await _service.UpdateAsync(id, parameter);
        return updated is null ? NotFound() : Ok(updated);
    }
}
