using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public interface IClStateRepository
{
    Task<IReadOnlyList<ClState>> GetAllAsync();
}
