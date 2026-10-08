using eKYC.Application.Dashboard;
using eKYC.Application.Tenancy;
using eKYC.Domain.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>"Nadzorna ploča" (dashboard) - the work queue of clients awaiting action.</summary>
/// <remarks>Backed by a multi-table join over client, processing-status and risk data rather than a single table.</remarks>
[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboard;
    private readonly ICurrentTenantProvider _tenantProvider;

    public DashboardController(IDashboardService dashboard, ICurrentTenantProvider tenantProvider)
    {
        ArgumentNullException.ThrowIfNull(dashboard);
        ArgumentNullException.ThrowIfNull(tenantProvider);
        _dashboard = dashboard;
        _tenantProvider = tenantProvider;
    }

    /// <summary>Lists clients in the work queue, filtered.</summary>
    /// <remarks>All filters are optional and combined with AND; omitted filters match everything.</remarks>
    /// <param name="clntTypCd">Client type code.</param>
    /// <param name="clntNm">Client name (partial match).</param>
    /// <param name="clntPrcsngSt">Processing status code.</param>
    /// <param name="oib">Croatian personal/company identification number.</param>
    /// <param name="rskEstId">Risk estimate id.</param>
    /// <response code="200">The matching clients.</response>
    [HttpGet("work-queue")]
    public async Task<ActionResult<IReadOnlyList<DashboardClientRow>>> GetWorkQueue(
        [FromQuery] string? clntTypCd,
        [FromQuery] string? clntNm,
        [FromQuery] string? clntPrcsngSt,
        [FromQuery] string? oib,
        [FromQuery] int? rskEstId)
    {
        var filter = new DashboardFilter
        {
            ClntTypCd = clntTypCd,
            ClntNm = clntNm,
            ClntPrcsngSt = clntPrcsngSt,
            Oib = oib,
            RskEstId = rskEstId,
        };

        var rows = await _dashboard.GetWorkQueueAsync(filter, _tenantProvider.GetCurrentTenantId());
        return Ok(rows);
    }
}
