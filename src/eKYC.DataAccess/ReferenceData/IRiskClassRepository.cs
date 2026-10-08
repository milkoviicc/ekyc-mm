using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public interface IRiskClassRepository
{
    Task<IReadOnlyList<RiskClass>> GetAllAsync();
}
