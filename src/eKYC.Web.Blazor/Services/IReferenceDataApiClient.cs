using eKYC.Domain.ReferenceData;
using eKYC.Domain.Revisions;

namespace eKYC.Web.Blazor.Services;

/// <summary>
/// Typed HTTP client for the reference-data endpoints on eKYC.Api. Components call this — never the
/// database or the DataAccess/Application layers directly.
/// </summary>
public interface IReferenceDataApiClient
{
    Task<IReadOnlyList<ClState>> GetStatesAsync();
    Task<IReadOnlyList<ClientType>> GetClientTypesAsync();
    Task<IReadOnlyList<RiskClass>> GetRiskClassesAsync();
    Task<IReadOnlyList<RiskEstimate>> GetRiskEstimatesAsync();
    Task<IReadOnlyList<OwnershipType>> GetOwnershipTypesAsync();
    Task<IReadOnlyList<DocumentType>> GetDocumentTypesAsync();
    Task<IReadOnlyList<ClientProcessingStatus>> GetProcessingStatusesAsync();
    Task<IReadOnlyList<RevisionType>> GetRevisionTypesAsync();
}
