using eKYC.Application.Revisions;
using eKYC.Application.Tenancy;
using eKYC.Domain.Revisions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>"Revizija" tab - document revisions (table CL_Doc_Revisions).</summary>
/// <remarks>
/// Reads require any authenticated user; writes require <c>UNOS</c>, <c>ADMIN</c> or <c>REVIZIJA</c>. Calls are tenant-scoped and
/// updates use optimistic locking (<c>row_version</c>, HTTP 409 on conflict).
/// </remarks>
[ApiController]
[Route("api/revisions")]
[Authorize]
public sealed class CL_Doc_RevisionsController : ControllerBase
{
    private readonly ICL_Doc_RevisionsService _revisions;
    private readonly ICurrentTenantProvider _tenantProvider;

    public CL_Doc_RevisionsController(ICL_Doc_RevisionsService revisions, ICurrentTenantProvider tenantProvider)
    {
        ArgumentNullException.ThrowIfNull(revisions);
        ArgumentNullException.ThrowIfNull(tenantProvider);
        _revisions = revisions;
        _tenantProvider = tenantProvider;
    }

    /// <summary>Lists revisions, optionally filtered.</summary>
    /// <param name="year">Only revisions of this year.</param>
    /// <param name="revTypeId">Only revisions of this type.</param>
    /// <response code="200">The revisions.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CL_Doc_Revisions>>> GetAll([FromQuery] int? year, [FromQuery] int? revTypeId)
    {
        var filter = new DocRevisionFilter { Year = year, RevTypeId = revTypeId };
        return Ok(await _revisions.GetAllAsync(filter, _tenantProvider.GetCurrentTenantId()));
    }

    /// <summary>Gets one revision.</summary>
    /// <param name="id">The revision id.</param>
    /// <response code="200">The revision.</response>
    /// <response code="404">No such revision in the current tenant.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CL_Doc_Revisions>> GetById(int id)
    {
        var revision = await _revisions.GetByIdAsync(id, _tenantProvider.GetCurrentTenantId());
        return revision is null ? NotFound() : Ok(revision);
    }

    /// <summary>Creates a revision.</summary>
    /// <response code="201">Created; the body is the stored revision.</response>
    [HttpPost]
    [Authorize(Roles = "UNOS,ADMIN,REVIZIJA")]
    public async Task<ActionResult<CL_Doc_Revisions>> Create(CL_Doc_Revisions revision)
    {
        ArgumentNullException.ThrowIfNull(revision);
        revision.tenant_id = _tenantProvider.GetCurrentTenantId();

        var newId = await _revisions.CreateAsync(revision);
        var created = await _revisions.GetByIdAsync(newId, revision.tenant_id);
        return CreatedAtAction(nameof(GetById), new { id = newId }, created);
    }

    /// <summary>Updates a revision.</summary>
    /// <param name="id">The revision id; must equal the body's <c>CL_Doc_Revision_Id</c>.</param>
    /// <param name="revision">The full revision with the <c>row_version</c> it was loaded with.</param>
    /// <response code="200">Updated.</response>
    /// <response code="400">Route id and body id differ.</response>
    /// <response code="404">No such revision in the current tenant.</response>
    /// <response code="409">Changed by someone else since it was loaded.</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "UNOS,ADMIN,REVIZIJA")]
    public async Task<IActionResult> Update(int id, CL_Doc_Revisions revision)
    {
        ArgumentNullException.ThrowIfNull(revision);
        if (id != revision.CL_Doc_Revision_Id)
        {
            return BadRequest("Route id does not match the submitted revision's CL_Doc_Revision_Id.");
        }

        revision.tenant_id = _tenantProvider.GetCurrentTenantId();

        var existing = await _revisions.GetByIdAsync(id, revision.tenant_id);
        if (existing is null)
        {
            return NotFound();
        }

        await _revisions.UpdateAsync(revision);
        return Ok();
    }

    /// <summary>Deletes a revision.</summary>
    /// <param name="id">The revision id.</param>
    /// <response code="204">Deleted.</response>
    /// <response code="404">No such revision in the current tenant.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "UNOS,ADMIN,REVIZIJA")]
    public async Task<IActionResult> Delete(int id)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        var existing = await _revisions.GetByIdAsync(id, tenantId);
        if (existing is null)
        {
            return NotFound();
        }

        await _revisions.DeleteAsync(id, tenantId);
        return NoContent();
    }
}
