using eKYC.Application.Clients;
using eKYC.Application.Tenancy;
using eKYC.Domain.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Clients - individuals, legal entities and correspondent banks (table CL_Clnt).</summary>
/// <remarks>
/// Reads require any authenticated user; writes require <c>UNOS</c> or <c>ADMIN</c>. Every call is scoped to the current tenant.
/// Updates use optimistic locking (<c>row_version</c>): a stale update fails with HTTP 409 carrying the current record.
/// </remarks>
[ApiController]
[Route("api/clients")]
[Authorize]
[RequireObject("tabClients", "tabOverview", "tabAdmin")]
public sealed class CL_ClntController : ControllerBase
{
    private readonly ICL_ClntService _clients;
    private readonly ICurrentTenantProvider _tenantProvider;

    public CL_ClntController(ICL_ClntService clients, ICurrentTenantProvider tenantProvider)
    {
        ArgumentNullException.ThrowIfNull(clients);
        ArgumentNullException.ThrowIfNull(tenantProvider);
        _clients = clients;
        _tenantProvider = tenantProvider;
    }

    /// <summary>Lists all clients of the current tenant.</summary>
    /// <response code="200">The clients.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CL_Clnt>>> GetAll() =>
        Ok(await _clients.GetAllAsync(_tenantProvider.GetCurrentTenantId()));

    /// <summary>Gets one client by <c>Clnt_Id</c>.</summary>
    /// <param name="id">The client id.</param>
    /// <response code="200">The client.</response>
    /// <response code="404">No such client in the current tenant.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CL_Clnt>> GetById(int id)
    {
        var client = await _clients.GetByIdAsync(id, _tenantProvider.GetCurrentTenantId());
        return client is null ? NotFound() : Ok(client);
    }

    /// <summary>Creates a client.</summary>
    /// <remarks>The tenant is taken from the server, not from the request body.</remarks>
    /// <response code="201">Created; the body is the stored client and <c>Location</c> points to it.</response>
    [HttpPost]
    [Authorize(Roles = "UNOS,ADMIN")] // matches the legacy A_ISRole codes (UNOS = data entry); ODOBR1/ODOBR2 approval gating is workstream 4
    public async Task<ActionResult<CL_Clnt>> Create(CL_Clnt client)
    {
        ArgumentNullException.ThrowIfNull(client);
        client.tenant_id = _tenantProvider.GetCurrentTenantId();

        var newId = await _clients.CreateAsync(client);
        var created = await _clients.GetByIdAsync(newId, client.tenant_id);
        return CreatedAtAction(nameof(GetById), new { id = newId }, created);
    }

    /// <summary>Updates a client.</summary>
    /// <remarks>The submitted <c>row_version</c> must match the stored one; otherwise 409 Conflict is returned with the freshly reloaded client.</remarks>
    /// <param name="id">The client id; must equal the body's <c>Clnt_Id</c>.</param>
    /// <param name="client">The full client with the <c>row_version</c> it was loaded with.</param>
    /// <response code="200">Updated.</response>
    /// <response code="400">Route id and body id differ.</response>
    /// <response code="404">No such client in the current tenant.</response>
    /// <response code="409">The client was changed by someone else since it was loaded.</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "UNOS,ADMIN")] // matches the legacy A_ISRole codes (UNOS = data entry); ODOBR1/ODOBR2 approval gating is workstream 4
    public async Task<IActionResult> Update(int id, CL_Clnt client)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (id != client.Clnt_Id)
        {
            return BadRequest("Route id does not match the submitted client's Clnt_Id.");
        }

        client.tenant_id = _tenantProvider.GetCurrentTenantId();

        var existing = await _clients.GetByIdAsync(id, client.tenant_id);
        if (existing is null)
        {
            return NotFound();
        }

        await _clients.UpdateAsync(client);
        return Ok();
    }
}
