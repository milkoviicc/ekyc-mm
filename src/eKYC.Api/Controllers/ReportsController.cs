using eKYC.Application.Reports;
using eKYC.Application.Tenancy;
using eKYC.Domain.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>"Izvješća" tab - risk reports over clients.</summary>
[ApiController]
[Route("api/reports")]
[Authorize]
[RequireObject("tabReports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportRiskService _reportRisk;
    private readonly ICurrentTenantProvider _tenantProvider;

    public ReportsController(IReportRiskService reportRisk, ICurrentTenantProvider tenantProvider)
    {
        ArgumentNullException.ThrowIfNull(reportRisk);
        ArgumentNullException.ThrowIfNull(tenantProvider);
        _reportRisk = reportRisk;
        _tenantProvider = tenantProvider;
    }

    /// <summary>Runs a risk report.</summary>
    /// <param name="reportKey">Which report to run; must be one of the known report keys.</param>
    /// <param name="addDtFrom">Created on or after.</param>
    /// <param name="addDtTo">Created on or before.</param>
    /// <param name="mdfDtFrom">Modified on or after.</param>
    /// <param name="mdfDtTo">Modified on or before.</param>
    /// <param name="clntTypCds">Restrict to these client type codes.</param>
    /// <param name="riskEstIds">Restrict to these risk estimate ids.</param>
    /// <response code="200">The report rows.</response>
    /// <response code="400"><c>reportKey</c> is missing or unknown.</response>
    [HttpGet("risk")]
    public async Task<ActionResult<IReadOnlyList<ReportRiskRow>>> GetRiskReport(
        [FromQuery] string reportKey,
        [FromQuery] DateTime? addDtFrom,
        [FromQuery] DateTime? addDtTo,
        [FromQuery] DateTime? mdfDtFrom,
        [FromQuery] DateTime? mdfDtTo,
        [FromQuery] string[]? clntTypCds,
        [FromQuery] int[]? riskEstIds)
    {
        if (string.IsNullOrWhiteSpace(reportKey) || !ReportKeys.All.Contains(reportKey))
        {
            return BadRequest($"reportKey must be one of: {string.Join(", ", ReportKeys.All)}.");
        }

        var filter = new ReportRiskFilter
        {
            AddDtFrom = addDtFrom,
            AddDtTo = addDtTo,
            MdfDtFrom = mdfDtFrom,
            MdfDtTo = mdfDtTo,
            ClntTypCds = clntTypCds,
            RiskEstIds = riskEstIds,
        };

        var rows = await _reportRisk.GetAsync(reportKey, filter, _tenantProvider.GetCurrentTenantId());
        return Ok(rows);
    }
}
