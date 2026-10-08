using eKYC.Application.ReferenceData;
using eKYC.Domain.ReferenceData;
using eKYC.Domain.Revisions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Read-only lookup tables used to populate dropdowns and filters.</summary>
/// <remarks>Each endpoint returns one whole lookup table; they are small and rarely change.</remarks>
[ApiController]
[Route("api/reference-data")]
[Authorize]
public sealed class ReferenceDataController : ControllerBase
{
    private readonly IReferenceDataService _referenceData;

    public ReferenceDataController(IReferenceDataService referenceData)
    {
        ArgumentNullException.ThrowIfNull(referenceData);
        _referenceData = referenceData;
    }

    /// <summary>Lists countries/states (table CL_States).</summary>
    [HttpGet("states")]
    public async Task<ActionResult<IReadOnlyList<ClState>>> GetStates() =>
        Ok(await _referenceData.GetStatesAsync());

    /// <summary>Lists client types (table CL_Clnt_Typ).</summary>
    [HttpGet("client-types")]
    public async Task<ActionResult<IReadOnlyList<ClientType>>> GetClientTypes() =>
        Ok(await _referenceData.GetClientTypesAsync());

    /// <summary>Lists risk classes (table CL_Rsk_Cls).</summary>
    [HttpGet("risk-classes")]
    public async Task<ActionResult<IReadOnlyList<RiskClass>>> GetRiskClasses() =>
        Ok(await _referenceData.GetRiskClassesAsync());

    /// <summary>Lists risk estimates (table CL_Rsk_Est).</summary>
    [HttpGet("risk-estimates")]
    public async Task<ActionResult<IReadOnlyList<RiskEstimate>>> GetRiskEstimates() =>
        Ok(await _referenceData.GetRiskEstimatesAsync());

    /// <summary>Lists ownership types (table CL_Ownrshp_Typ).</summary>
    [HttpGet("ownership-types")]
    public async Task<ActionResult<IReadOnlyList<OwnershipType>>> GetOwnershipTypes() =>
        Ok(await _referenceData.GetOwnershipTypesAsync());

    /// <summary>Lists document types (table CL_Doc_Typ).</summary>
    [HttpGet("document-types")]
    public async Task<ActionResult<IReadOnlyList<DocumentType>>> GetDocumentTypes() =>
        Ok(await _referenceData.GetDocumentTypesAsync());

    /// <summary>Lists client processing statuses (table CL_Clnt_PrcsSt).</summary>
    [HttpGet("processing-statuses")]
    public async Task<ActionResult<IReadOnlyList<ClientProcessingStatus>>> GetProcessingStatuses() =>
        Ok(await _referenceData.GetClientProcessingStatusesAsync());

    /// <summary>Lists the allowed processing-status transitions (table CL_Clnt_PrcsSt_To).</summary>
    [HttpGet("processing-status-transitions")]
    public async Task<ActionResult<IReadOnlyList<ClientProcessingStatusTransition>>> GetProcessingStatusTransitions() =>
        Ok(await _referenceData.GetClientProcessingStatusTransitionsAsync());

    /// <summary>Lists revision types (table CL_Rev_Typ).</summary>
    [HttpGet("revision-types")]
    public async Task<ActionResult<IReadOnlyList<RevisionType>>> GetRevisionTypes() =>
        Ok(await _referenceData.GetRevisionTypesAsync());
}
