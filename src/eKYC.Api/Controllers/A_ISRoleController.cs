using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Information-system roles (table A_ISRole), e.g. UNOS, ODOBR1, ODOBR2, ADMIN.</summary>
/// <remarks>
/// Restricted to the <c>ADMIN</c> role, read-only for now. <c>ISRol_St</c> holds the role status code (lookup A_ISRole_St).
/// </remarks>
[ApiController]
[Route("api/admin/roles")]
[Authorize(Roles = "ADMIN")]
public sealed class A_ISRoleController : ControllerBase
{
    private readonly IA_ISRoleService _service;

    public A_ISRoleController(IA_ISRoleService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    /// <summary>Lists all roles.</summary>
    /// <response code="200">The list of rows; empty when the table has none.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<A_ISRole>>> GetAll() => Ok(await _service.GetAllAsync());

    /// <summary>Gets one role by <c>ISRol_Id</c>.</summary>
    /// <param name="id">The primary key of the row.</param>
    /// <response code="200">The row.</response>
    /// <response code="404">No row with that id.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<A_ISRole>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }
}
