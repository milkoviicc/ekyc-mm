using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Audit trail of changes (table Operation_Log), newest first.</summary>
/// <remarks>Restricted to the <c>ADMIN</c> role and read-only. Changes made through the admin endpoints are written here automatically.</remarks>
[ApiController]
[Route("api/admin/audit")]
[Authorize(Roles = "ADMIN")]
[RequireObject("tabAdmin")]
public sealed class Operation_LogController : ControllerBase
{
    private readonly IOperation_LogService _service;

    public Operation_LogController(IOperation_LogService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    /// <summary>Lists audit entries.</summary>
    /// <remarks>All filters are optional and combined with AND. <c>to</c> includes that whole day. At most 5000 rows are returned.</remarks>
    /// <param name="from">Entries on or after this day.</param>
    /// <param name="to">Entries on or before this day.</param>
    /// <param name="userId">Only entries by this user (A_Usr.Usr_Id).</param>
    /// <param name="objectType">Only this object type, e.g. <c>A_USR</c> or <c>CLNT_PHS</c>.</param>
    /// <param name="take">Maximum rows (default 500).</param>
    /// <response code="200">The entries, joined to the actor's name.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Operation_Log>>> Query(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int? userId,
        [FromQuery] string? objectType,
        [FromQuery] int take = 500)
    {
        var filter = new OperationLogFilter { From = from, To = to, User_Id = userId, Object_Type = objectType, Take = take };
        return Ok(await _service.QueryAsync(filter));
    }

    /// <summary>Lists the distinct object types found in the log, for a filter dropdown.</summary>
    /// <response code="200">The object types.</response>
    [HttpGet("object-types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetObjectTypes() => Ok(await _service.GetObjectTypesAsync());
}
