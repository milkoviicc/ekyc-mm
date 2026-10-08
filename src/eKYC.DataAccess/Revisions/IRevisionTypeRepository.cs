using eKYC.Domain.Revisions;

namespace eKYC.DataAccess.Revisions;

public interface IRevisionTypeRepository
{
    Task<IReadOnlyList<RevisionType>> GetAllAsync();
}
