using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public interface IClientTypeRepository
{
    Task<IReadOnlyList<ClientType>> GetAllAsync();
}
