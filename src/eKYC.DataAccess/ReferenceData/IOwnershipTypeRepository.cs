using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public interface IOwnershipTypeRepository
{
    Task<IReadOnlyList<OwnershipType>> GetAllAsync();
}
