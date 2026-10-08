using eKYC.Domain.ReferenceData;
using eKYC.Domain.Revisions;

namespace eKYC.Application.ReferenceData;

/// <summary>
/// Read-only access to the small lookup tables (states, client types, risk classes, etc.) that back
/// dropdowns throughout the client onboarding screens. Grouped into one service rather than one per
/// table since none of them carry any business logic beyond "fetch and return".
/// </summary>
public interface IReferenceDataService
{
    Task<IReadOnlyList<ClState>> GetStatesAsync();
    Task<IReadOnlyList<ClientType>> GetClientTypesAsync();
    Task<IReadOnlyList<RiskClass>> GetRiskClassesAsync();
    Task<IReadOnlyList<RiskEstimate>> GetRiskEstimatesAsync();
    Task<IReadOnlyList<OwnershipType>> GetOwnershipTypesAsync();
    Task<IReadOnlyList<DocumentType>> GetDocumentTypesAsync();
    Task<IReadOnlyList<ClientProcessingStatus>> GetClientProcessingStatusesAsync();
    Task<IReadOnlyList<ClientProcessingStatusTransition>> GetClientProcessingStatusTransitionsAsync();
    Task<IReadOnlyList<RevisionType>> GetRevisionTypesAsync();
}
