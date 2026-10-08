using eKYC.DataAccess.ReferenceData;
using eKYC.DataAccess.Revisions;
using eKYC.Domain.ReferenceData;
using eKYC.Domain.Revisions;

namespace eKYC.Application.ReferenceData;

public sealed class ReferenceDataService : IReferenceDataService
{
    private readonly IClStateRepository _states;
    private readonly IClientTypeRepository _clientTypes;
    private readonly IRiskClassRepository _riskClasses;
    private readonly IRiskEstimateRepository _riskEstimates;
    private readonly IOwnershipTypeRepository _ownershipTypes;
    private readonly IDocumentTypeRepository _documentTypes;
    private readonly IClientProcessingStatusRepository _processingStatuses;
    private readonly IRevisionTypeRepository _revisionTypes;

    public ReferenceDataService(
        IClStateRepository states,
        IClientTypeRepository clientTypes,
        IRiskClassRepository riskClasses,
        IRiskEstimateRepository riskEstimates,
        IOwnershipTypeRepository ownershipTypes,
        IDocumentTypeRepository documentTypes,
        IClientProcessingStatusRepository processingStatuses,
        IRevisionTypeRepository revisionTypes)
    {
        ArgumentNullException.ThrowIfNull(states);
        ArgumentNullException.ThrowIfNull(clientTypes);
        ArgumentNullException.ThrowIfNull(riskClasses);
        ArgumentNullException.ThrowIfNull(riskEstimates);
        ArgumentNullException.ThrowIfNull(ownershipTypes);
        ArgumentNullException.ThrowIfNull(documentTypes);
        ArgumentNullException.ThrowIfNull(processingStatuses);
        ArgumentNullException.ThrowIfNull(revisionTypes);

        _states = states;
        _clientTypes = clientTypes;
        _riskClasses = riskClasses;
        _riskEstimates = riskEstimates;
        _ownershipTypes = ownershipTypes;
        _documentTypes = documentTypes;
        _processingStatuses = processingStatuses;
        _revisionTypes = revisionTypes;
    }

    public Task<IReadOnlyList<ClState>> GetStatesAsync() => _states.GetAllAsync();

    public Task<IReadOnlyList<ClientType>> GetClientTypesAsync() => _clientTypes.GetAllAsync();

    public Task<IReadOnlyList<RiskClass>> GetRiskClassesAsync() => _riskClasses.GetAllAsync();

    public Task<IReadOnlyList<RiskEstimate>> GetRiskEstimatesAsync() => _riskEstimates.GetAllAsync();

    public Task<IReadOnlyList<OwnershipType>> GetOwnershipTypesAsync() => _ownershipTypes.GetAllAsync();

    public Task<IReadOnlyList<DocumentType>> GetDocumentTypesAsync() => _documentTypes.GetAllAsync();

    public Task<IReadOnlyList<ClientProcessingStatus>> GetClientProcessingStatusesAsync() =>
        _processingStatuses.GetAllAsync();

    public Task<IReadOnlyList<ClientProcessingStatusTransition>> GetClientProcessingStatusTransitionsAsync() =>
        _processingStatuses.GetAllTransitionsAsync();

    public Task<IReadOnlyList<RevisionType>> GetRevisionTypesAsync() => _revisionTypes.GetAllAsync();
}
