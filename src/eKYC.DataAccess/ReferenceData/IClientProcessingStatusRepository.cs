using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public interface IClientProcessingStatusRepository
{
    Task<IReadOnlyList<ClientProcessingStatus>> GetAllAsync();

    Task<IReadOnlyList<ClientProcessingStatusTransition>> GetAllTransitionsAsync();
}
