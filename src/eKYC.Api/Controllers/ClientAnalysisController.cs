using eKYC.Application.Dashboard;
using eKYC.Application.Tenancy;
using eKYC.Domain.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>"Klijenti - analiza" tab - clients of one type, for analysis.</summary>
/// <remarks>Shares its row shape and filters with the dashboard work queue.</remarks>
[ApiController]
[Route("api/client-analysis")]
[Authorize]
[RequireObject("tabClients", "tabOverview")]
public sealed class ClientAnalysisController : ControllerBase
{
    private readonly IClientAnalysisService _clientAnalysis;
    private readonly ICurrentTenantProvider _tenantProvider;

    public ClientAnalysisController(IClientAnalysisService clientAnalysis, ICurrentTenantProvider tenantProvider)
    {
        ArgumentNullException.ThrowIfNull(clientAnalysis);
        ArgumentNullException.ThrowIfNull(tenantProvider);
        _clientAnalysis = clientAnalysis;
        _tenantProvider = tenantProvider;
    }

    /// <summary>Lists clients of the given type, filtered.</summary>
    /// <param name="clntTypCd">Client type code (required).</param>
    /// <param name="clntNm">Client name (partial match).</param>
    /// <param name="clntPrcsngSt">Processing status code.</param>
    /// <param name="oib">Croatian personal/company identification number.</param>
    /// <param name="rskEstId">Risk estimate id.</param>
    /// <response code="200">The matching clients.</response>
    /// <response code="400"><c>clntTypCd</c> is missing.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DashboardClientRow>>> GetByType(
        [FromQuery] string clntTypCd,
        [FromQuery] string? clntNm,
        [FromQuery] string? clntPrcsngSt,
        [FromQuery] string? oib,
        [FromQuery] int? rskEstId)
    {
        if (string.IsNullOrWhiteSpace(clntTypCd))
        {
            return BadRequest("clntTypCd is required.");
        }

        var filter = new DashboardFilter
        {
            ClntNm = clntNm,
            ClntPrcsngSt = clntPrcsngSt,
            Oib = oib,
            RskEstId = rskEstId,
        };

        var rows = await _clientAnalysis.GetByTypeAsync(clntTypCd, filter, _tenantProvider.GetCurrentTenantId());
        return Ok(rows);
    }
}
