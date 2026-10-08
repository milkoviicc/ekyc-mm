using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public interface IRiskEstimateRepository
{
    Task<IReadOnlyList<RiskEstimate>> GetAllAsync();
}
