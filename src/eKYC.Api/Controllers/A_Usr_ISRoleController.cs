using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>User-to-role assignments with a validity window (table A_Usr_ISRole).</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role, read-only for now. A user can hold several roles; an assignment applies between <c>Vld_From_Dt</c> and <c>Vld_To_Dt</c>.
/// </remarks>
[ApiController]
[Route("api/admin/user-roles")]
[Authorize(Roles = "ADMIN")]
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
}
